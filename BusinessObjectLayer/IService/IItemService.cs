using BusinessObjectLayer.DTOs.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.IService
{
    public interface IItemService
    {
        Task<List<ItemDto>> GetAllItemsAsync();
        Task<ItemDto?> GetItemByIdAsync(Guid id);
        Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto);
        Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto);
        Task<bool> DeleteItemAsync(Guid id);
        Task<List<ItemDto>> GetItemsByStatusAsync(string status);
        Task<List<ItemDto>> GetItemsByUserIdAsync(Guid userId);
        Task<List<ItemDto>> GetItemsByCategoryIdAsync(Guid categoryId);
        Task<List<ItemDto>> GetItemsByLocationIdAsync(Guid locationId);
        Task<List<ItemDto>> SearchItemsAsync(string searchTerm);
        Task<List<ItemDto>> GetItemsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}
