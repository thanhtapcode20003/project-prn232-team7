using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.DTOs.Common;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/items")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IItemService itemService, ILogger<ItemController> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/items - Get all items with pagination and filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllItems([FromQuery] ItemFilterDto filter)
    {
        var result = await _itemService.GetAllItemsAsync(filter);
        return Ok(ApiResponse<PagedResult<ItemDto>>.Ok(result, "Items retrieved successfully"));
    }

    /// <summary>
    /// GET /api/items/{id} - Get item by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItemById(int id)
    {
        var item = await _itemService.GetItemByIdAsync(id);
        return Ok(ApiResponse<ItemDto>.Ok(item, "Item retrieved successfully"));
    }

    /// <summary>
    /// POST /api/items - Create new item (requires authentication)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            throw new ValidationException(errors);
        }

        // Get current user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? currentUserId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
        {
            currentUserId = userId;
        }

        var item = await _itemService.CreateItemAsync(request, currentUserId);
        
        return CreatedAtAction(
            nameof(GetItemById),
            new { id = item.Id },
            ApiResponse<ItemDto>.Ok(item, "Item created successfully")
        );
    }

    /// <summary>
    /// PUT /api/items/{id} - Update item (requires authentication)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            throw new ValidationException(errors);
        }

        var item = await _itemService.UpdateItemAsync(id, request);
        return Ok(ApiResponse<ItemDto>.Ok(item, "Item updated successfully"));
    }

    /// <summary>
    /// DELETE /api/items/{id} - Delete item (soft delete, requires authentication)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await _itemService.DeleteItemAsync(id);
        return Ok(ApiResponse<object>.Ok(null, "Item deleted successfully"));
    }

    /// <summary>
    /// GET /api/items/search - Search items with filters
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchItems([FromQuery] ItemFilterDto filter)
    {
        var result = await _itemService.SearchItemsAsync(filter);
        return Ok(ApiResponse<PagedResult<ItemDto>>.Ok(result, "Items searched successfully"));
    }
}