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

            // Generate file URL
            var fileUrl = $"/{UploadFolder}/{fileName}";
            var checkCate = await _context.Categories.FindAsync(createItemDto.CategoryId);
            if (checkCate == null)
            {
                throw new NotFoundException("CategoryId not found");
            }
            var checkLocation = await _context.ServiceLocations.FindAsync(createItemDto.CurrentLocationId);
            if (checkLocation == null)
            {
                throw new NotFoundException("CurrentLocationId not found");
            }
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Img = fileName, // ✅ lưu path hoặc filename
                CategoryId = checkCate.Id,
                Status = StatusEnum.ACTIVE.ToString(),
                Date = DateTime.UtcNow,
                FoundLocation = createItemDto.FoundLocation,
                CurrentLocationId = checkLocation.Id,
                Context = createItemDto.Context,
                UserId = user.Id,
                FoundDate = createItemDto.FoundDate
            };

            // 7. Save DB
            await _itemRepository.CreateAsync(item);

            var createdItem = await _itemRepository.GetByIdWithDetailsAsync(item.Id);

            return MapToDto(createdItem!);
        }

        public async Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemRepository.GetByIdAsync(id);
            if (existingItem == null)
                return null;
            var user = GetCurrentUser();
            if (!user.Equals(existingItem.User))
            {
                throw new UnauthorizedException("it not of you");
            }
            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Img = updateItemDto.Img;
            existingItem.CategoryId = updateItemDto.CategoryId;

            existingItem.FoundLocation = updateItemDto.FoundLocation;
            existingItem.CurrentLocationId = updateItemDto.CurrentLocationId;
            existingItem.Context = updateItemDto.Context;
            existingItem.FoundDate = updateItemDto.FoundDate;

            await _itemRepository.UpdateAsync(existingItem);
            var updatedItem = await _itemRepository.GetByIdWithDetailsAsync(id);
            return MapToDto(updatedItem!);
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            var user = GetCurrentUser();
            if (!user.Equals(item.User))
            {
                throw new UnauthorizedException("it not of you");
            }
            if (item == null)
                return false;

            await _itemRepository.RemoveAsync(item);
            return true;
        }

        // ✅ Unified Search Method with Pagination
        public async Task<PagedResult<ItemDto>> SearchItemsAsync(ItemFilterDto filter)
        {
            var items = await _itemRepository.SearchItemsAsync(

                categoryId: filter.CategoryId,
                locationId: filter.LocationId,
                searchTerm: filter.SearchTerm,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _itemRepository.CountItemsAsync(

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

        // ✅ Single MapToDto method
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
                UserId = item.UserId,
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