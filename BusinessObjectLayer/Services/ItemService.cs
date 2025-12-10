using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

            // Get the created item with details
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

            // Get the updated item with details
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

        public async Task<List<ItemDto>> GetItemsByStatusAsync(string status)
        {
            var items = await _itemRepository.GetByStatusAsync(status);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ItemDto>> GetItemsByUserIdAsync(Guid userId)
        {
            var items = await _itemRepository.GetByUserIdAsync(userId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ItemDto>> GetItemsByCategoryIdAsync(Guid categoryId)
        {
            var items = await _itemRepository.GetByCategoryIdAsync(categoryId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ItemDto>> GetItemsByLocationIdAsync(Guid locationId)
        {
            var items = await _itemRepository.GetByLocationIdAsync(locationId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ItemDto>> SearchItemsAsync(string searchTerm)
        {
            var items = await _itemRepository.SearchByNameAsync(searchTerm);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ItemDto>> GetItemsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            var items = await _itemRepository.GetByDateRangeAsync(startDate, endDate);
            return items.Select(MapToDto).ToList();
        }

        // Helper method to map Entity to DTO
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
