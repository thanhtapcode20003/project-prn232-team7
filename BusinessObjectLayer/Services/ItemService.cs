using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Security.Claims;

namespace BusinessObjectLayer.Services
{
    public class ItemService : IItemService
    {
        private readonly ItemRepository _itemRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LostAndFoundDbContext _context;
        private const string UploadFolder = "uploads";
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        public ItemService()
        {
            _itemRepository = new ItemRepository();
        }

        public ItemService(ItemRepository itemRepository, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, LostAndFoundDbContext lostAndFoundDbContext)
        {
            _itemRepository = itemRepository;
            _environment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _context = lostAndFoundDbContext;
        }

        public async Task<List<ItemDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllWithDetailsAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<ItemDto?> GetItemByIdAsync(Guid id)
        {
            var item = await _itemRepository.GetByIdWithDetailsAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto, IFormFile file)
        {

            var user = GetCurrentUser();

            ValidateFile(file);

            // Validate Category exists
            var checkCate = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == createItemDto.CategoryId && c.Status == StatusEnum.ACTIVE.ToString());
            if (checkCate == null)
            {
                throw new NotFoundException("Category", createItemDto.CategoryId.ToString());
            }

            // Validate Location if provided
            ServiceLocation? checkLocation = null;
            if (createItemDto.CurrentLocationId.HasValue)
            {
                checkLocation = await _context.ServiceLocations
                    .FirstOrDefaultAsync(l => l.Id == createItemDto.CurrentLocationId.Value && l.Status == StatusEnum.ACTIVE.ToString());
                if (checkLocation == null)
                {
                    throw new NotFoundException("ServiceLocation", createItemDto.CurrentLocationId.Value.ToString());
                }
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Img = fileName,
                CategoryId = checkCate.Id,
                Status = StatusEnum.ACTIVE.ToString(),
                Date = DateTime.UtcNow,
                FoundLocation = createItemDto.FoundLocation,
                CurrentLocationId = checkLocation?.Id,
                Context = createItemDto.Context,
                UserId = user.Id,
                FoundDate = createItemDto.FoundDate
            };

            await _itemRepository.CreateAsync(item);

            var createdItem = await _itemRepository.GetByIdWithDetailsAsync(item.Id);

            return MapToDto(createdItem!);
        }

        public async Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemRepository.GetByIdAsync(id);
            if (existingItem == null)
                throw new NotFoundException("Item", id.ToString());

            var user = GetCurrentUser();
            
            // Check authorization
            if (existingItem.UserId.HasValue && existingItem.UserId != user.Id)
            {
                throw new UnauthorizedException("You do not have permission to update this item");
            }

            // Validate Category exists
            var checkCate = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == updateItemDto.CategoryId && c.Status == StatusEnum.ACTIVE.ToString());
            if (checkCate == null)
            {
                throw new NotFoundException("Category", updateItemDto.CategoryId.ToString());
            }

            // Validate Location if provided
            if (updateItemDto.CurrentLocationId.HasValue)
            {
                var checkLocation = await _context.ServiceLocations
                    .FirstOrDefaultAsync(l => l.Id == updateItemDto.CurrentLocationId.Value && l.Status == StatusEnum.ACTIVE.ToString());
                if (checkLocation == null)
                {
                    throw new NotFoundException("ServiceLocation", updateItemDto.CurrentLocationId.Value.ToString());
                }
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.CategoryId = updateItemDto.CategoryId;
            existingItem.FoundLocation = updateItemDto.FoundLocation;
            existingItem.CurrentLocationId = updateItemDto.CurrentLocationId;
            existingItem.FoundDate = updateItemDto.FoundDate;

            await _itemRepository.UpdateAsync(existingItem);
            var updatedItem = await _itemRepository.GetByIdWithDetailsAsync(id);
            return MapToDto(updatedItem!);
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new NotFoundException("Item", id.ToString());

            var user = GetCurrentUser();
            
            // Check authorization
            if (item.UserId.HasValue && item.UserId != user.Id)
            {
                throw new UnauthorizedException("You do not have permission to delete this item");
            }

            await _itemRepository.RemoveAsync(item);
            return true;
        }

        public async Task<PagedResult<ItemDto>> SearchItemsAsync(ItemFilterDto filter)
        {
            var items = await _itemRepository.SearchItemsAsync(
                userId: filter.UserId,
                categoryId: filter.CategoryId,
                locationId: filter.LocationId,
                searchTerm: filter.SearchTerm,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _itemRepository.CountItemsAsync(
                userId: filter.UserId,
                categoryId: filter.CategoryId,
                locationId: filter.LocationId,
                searchTerm: filter.SearchTerm,
                fromDate: filter.FromDate,
                toDate: filter.ToDate
            );

            return new PagedResult<ItemDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required and cannot be empty");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }
        }

        private ItemDto MapToDto(Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Img = item.Img,
                CategoryId = item.CategoryId,
                Status = item.Status,
                Date = item.Date,
                FoundLocation = item.FoundLocation,
                CurrentLocationId = item.CurrentLocationId,
                Context = item.Context,
                UserId = item.UserId ?? Guid.Empty,
                FoundDate = item.FoundDate,
                CategoryName = item.Category?.Name,
                CurrentLocationName = item.CurrentLocation?.Name,
                UserName = item.User?.Username
            };
        }

        public User GetCurrentUser()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                  .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("User not authenticated");
            }
            var userId = Guid.Parse(userIdClaim);
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new UnauthorizedException("User not found");
            }
            return user;
        }
    }
}