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
                ItemId = Guid.NewGuid(),
                ItemName = createItemDto.ItemName,
                Description = createItemDto.Description,
                LostDate = createItemDto.LostDate,
                LostTime = createItemDto.LostTime,
                CategoryId = createItemDto.CategoryId,
                UserId = createItemDto.UserId,
                LocationId = createItemDto.LocationId,
                Status = createItemDto.Status
            };

            await _itemRepository.CreateAsync(item);
            var createdItem = await _itemRepository.GetByIdWithDetailsAsync(item.ItemId);
            return MapToDto(createdItem!);
        }

        public async Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _itemRepository.GetByIdAsync(id);
            if (existingItem == null)
                return null;

            existingItem.ItemName = updateItemDto.ItemName;
            existingItem.Description = updateItemDto.Description;
            existingItem.LostDate = updateItemDto.LostDate;
            existingItem.LostTime = updateItemDto.LostTime;
            existingItem.CategoryId = updateItemDto.CategoryId;
            existingItem.UserId = updateItemDto.UserId;
            existingItem.LocationId = updateItemDto.LocationId;
            existingItem.Status = updateItemDto.Status;

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
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                Description = item.Description,
                LostDate = item.LostDate,
                LostTime = item.LostTime,
                CategoryId = item.CategoryId,
                UserId = item.UserId,
                LocationId = item.LocationId,
                Status = item.Status,
                CategoryName = item.Category?.CategoryName,
                LocationName = item.Location?.LocationName,
                UserName = item.User?.Username
            };
        }
    }
}