using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.DTOs.Common;

namespace BusinessObjectLayer.IService;

public interface IItemService
{
    Task<PagedResult<ItemDto>> GetAllItemsAsync(ItemFilterDto filter);
    Task<ItemDto> GetItemByIdAsync(int id);
    Task<ItemDto> CreateItemAsync(CreateItemRequestDto request, int? currentUserId = null);
    Task<ItemDto> UpdateItemAsync(int id, UpdateItemRequestDto request);
    Task DeleteItemAsync(int id);
    Task<PagedResult<ItemDto>> SearchItemsAsync(ItemFilterDto filter);
}