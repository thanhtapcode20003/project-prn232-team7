using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;

namespace BusinessObjectLayer.Services
{
    public class ItemService : IItemService
    {
        private readonly ItemRepository _itemRepository;

        public ItemService()
        {
            _itemRepository = new ItemRepository();
        }

        public ItemService(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
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

        public async Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Img = createItemDto.Img,
                CategoryId = createItemDto.CategoryId,
                Status = createItemDto.Status,
                Date = createItemDto.Date,
                FoundLocation = createItemDto.FoundLocation,
                CurrentLocationId = createItemDto.CurrentLocationId,
                Context = createItemDto.Context,
                UserId = createItemDto.UserId,
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
                return null;

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Img = updateItemDto.Img;
            existingItem.CategoryId = updateItemDto.CategoryId;
            existingItem.Status = updateItemDto.Status;
            existingItem.Date = updateItemDto.Date;
            existingItem.FoundLocation = updateItemDto.FoundLocation;
            existingItem.CurrentLocationId = updateItemDto.CurrentLocationId;
            existingItem.Context = updateItemDto.Context;
            existingItem.UserId = updateItemDto.UserId;
            existingItem.FoundDate = updateItemDto.FoundDate;

            await _itemRepository.UpdateAsync(existingItem);
            var updatedItem = await _itemRepository.GetByIdWithDetailsAsync(id);
            return MapToDto(updatedItem!);
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                return false;

            await _itemRepository.RemoveAsync(item);
            return true;
        }

        // ✅ Unified Search Method with Pagination
        public async Task<PagedResult<ItemDto>> SearchItemsAsync(ItemFilterDto filter)
        {
            var items = await _itemRepository.SearchItemsAsync(
                status: filter.Status,
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
                status: filter.Status,
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

        // ✅ Single MapToDto method
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
    }
}