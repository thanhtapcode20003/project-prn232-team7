using BusinessObjectLayer.DTOs.Item;
using Microsoft.AspNetCore.Http;

namespace BusinessObjectLayer.IService
{
    public interface IItemService
    {
        Task<List<ItemDto>> GetAllItemsAsync();
        Task<ItemDto?> GetItemByIdAsync(Guid id);
        Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto, IFormFile file);
        Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto);
        Task<bool> DeleteItemAsync(Guid id);

        // ✅ Method mới
        Task<PagedResult<ItemDto>> SearchItemsAsync(ItemFilterDto filter);
    }

    // ✅ Response cho pagination
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}
