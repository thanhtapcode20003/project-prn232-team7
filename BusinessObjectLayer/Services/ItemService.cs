using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.DTOs.Common;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessObjectLayer.Services;

public class ItemService : IItemService
{
    private readonly LostAndFoundSystemDbContext _context;
    private readonly ILogger<ItemService> _logger;

    public ItemService(LostAndFoundSystemDbContext context, ILogger<ItemService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ItemDto>> GetAllItemsAsync(ItemFilterDto filter)
    {
        var query = _context.Items
            .Include(i => i.Category)
            .Include(i => i.CurrentLocation)
            .Include(i => i.User)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, filter);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, filter);

        // Apply pagination
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => MapToDto(i))
            .ToListAsync();

        return new PagedResult<ItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<ItemDto> GetItemByIdAsync(int id)
    {
        var item = await _context.Items
            .Include(i => i.Category)
            .Include(i => i.CurrentLocation)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            throw new NotFoundException($"Item with ID {id}");
        }

        return MapToDto(item);
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemRequestDto request, int? currentUserId = null)
    {
        // Validate CategoryId
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new NotFoundException($"Category with ID {request.CategoryId}");
        }

        // Validate CurrentLocationId if provided
        if (request.CurrentLocationId.HasValue)
        {
            var locationExists = await _context.ServiceLocations
                .AnyAsync(l => l.Id == request.CurrentLocationId.Value);
            if (!locationExists)
            {
                throw new NotFoundException($"Service Location with ID {request.CurrentLocationId}");
            }
        }

        // Validate UserId if provided
        if (request.UserId.HasValue)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId.Value);
            if (!userExists)
            {
                throw new NotFoundException($"User with ID {request.UserId}");
            }
        }

        var item = new DataAccessLayer.Models.Item
        {
            Name = request.Name,
            Description = request.Description,
            Img = request.Img,
            CategoryId = request.CategoryId,
            FoundLocation = request.FoundLocation,
            CurrentLocationId = request.CurrentLocationId,
            Content = request.Content,
            UserId = request.UserId ?? currentUserId,
            FoundDate = request.FoundDate ?? DateTime.UtcNow,
            Date = DateTime.UtcNow,
            Status = 1 // Active by default
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(item).Reference(i => i.Category).LoadAsync();
        if (item.CurrentLocationId.HasValue)
            await _context.Entry(item).Reference(i => i.CurrentLocation).LoadAsync();
        if (item.UserId.HasValue)
            await _context.Entry(item).Reference(i => i.User).LoadAsync();

        _logger.LogInformation("Item created with ID: {ItemId}", item.Id);

        return MapToDto(item);
    }

    public async Task<ItemDto> UpdateItemAsync(int id, UpdateItemRequestDto request)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            throw new NotFoundException($"Item with ID {id}");
        }

        // Validate CategoryId if provided
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId.Value);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {request.CategoryId}");
            }
            item.CategoryId = request.CategoryId.Value;
        }

        // Validate CurrentLocationId if provided
        if (request.CurrentLocationId.HasValue)
        {
            var locationExists = await _context.ServiceLocations
                .AnyAsync(l => l.Id == request.CurrentLocationId.Value);
            if (!locationExists)
            {
                throw new NotFoundException($"Service Location with ID {request.CurrentLocationId}");
            }
            item.CurrentLocationId = request.CurrentLocationId;
        }

        // Validate UserId if provided
        if (request.UserId.HasValue)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId.Value);
            if (!userExists)
            {
                throw new NotFoundException($"User with ID {request.UserId}");
            }
            item.UserId = request.UserId;
        }

        // Update fields
        if (request.Name != null) item.Name = request.Name;
        if (request.Description != null) item.Description = request.Description;
        if (request.Img != null) item.Img = request.Img;
        if (request.Status.HasValue) item.Status = request.Status.Value;
        if (request.FoundLocation != null) item.FoundLocation = request.FoundLocation;
        if (request.Content != null) item.Content = request.Content;
        if (request.FoundDate.HasValue) item.FoundDate = request.FoundDate;

        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(item).Reference(i => i.Category).LoadAsync();
        if (item.CurrentLocationId.HasValue)
            await _context.Entry(item).Reference(i => i.CurrentLocation).LoadAsync();
        if (item.UserId.HasValue)
            await _context.Entry(item).Reference(i => i.User).LoadAsync();

        _logger.LogInformation("Item updated with ID: {ItemId}", item.Id);

        return MapToDto(item);
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            throw new NotFoundException($"Item with ID {id}");
        }

        // Soft delete
        item.Status = 0;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Item soft deleted with ID: {ItemId}", item.Id);
    }

    public async Task<PagedResult<ItemDto>> SearchItemsAsync(ItemFilterDto filter)
    {
        return await GetAllItemsAsync(filter);
    }

    private static IQueryable<DataAccessLayer.Models.Item> ApplyFilters(
        IQueryable<DataAccessLayer.Models.Item> query, ItemFilterDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(i =>
                (i.Name != null && i.Name.ToLower().Contains(searchTerm)) ||
                (i.Description != null && i.Description.ToLower().Contains(searchTerm)) ||
                (i.Content != null && i.Content.ToLower().Contains(searchTerm)));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == filter.CategoryId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(i => i.Status == filter.Status.Value);
        }

        if (filter.CurrentLocationId.HasValue)
        {
            query = query.Where(i => i.CurrentLocationId == filter.CurrentLocationId.Value);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(i => i.UserId == filter.UserId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(i => i.Date >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(i => i.Date <= filter.ToDate.Value);
        }

        return query;
    }

    private static IQueryable<DataAccessLayer.Models.Item> ApplySorting(
        IQueryable<DataAccessLayer.Models.Item> query, ItemFilterDto filter)
    {
        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.SortDescending
                ? query.OrderByDescending(i => i.Name)
                : query.OrderBy(i => i.Name),
            "founddate" => filter.SortDescending
                ? query.OrderByDescending(i => i.FoundDate)
                : query.OrderBy(i => i.FoundDate),
            "category" => filter.SortDescending
                ? query.OrderByDescending(i => i.Category.Name)
                : query.OrderBy(i => i.Category.Name),
            _ => filter.SortDescending
                ? query.OrderByDescending(i => i.Date)
                : query.OrderBy(i => i.Date)
        };

        return query;
    }

    private static ItemDto MapToDto(DataAccessLayer.Models.Item item)
    {
        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Img = item.Img,
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name,
            Status = item.Status,
            Date = item.Date,
            FoundLocation = item.FoundLocation,
            CurrentLocationId = item.CurrentLocationId,
            CurrentLocationName = item.CurrentLocation?.Name,
            Content = item.Content,
            UserId = item.UserId,
            UserName = item.User?.Name,
            FoundDate = item.FoundDate
        };
    }
}