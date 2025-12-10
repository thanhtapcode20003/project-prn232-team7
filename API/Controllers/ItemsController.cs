using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IItemService itemService, ILogger<ItemsController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        /// <summary>
        /// Get all items
        /// </summary>
        /// <returns>List of all items</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetAllItems()
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items");
                return StatusCode(500, new { message = "An error occurred while retrieving items" });
            }
        }

        /// <summary>
        /// Get item by ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Item details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ItemDto>> GetItemById(Guid id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                    return NotFound(new { message = $"Item with ID {id} not found" });

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item {ItemId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the item" });
            }
        }

        /// <summary>
        /// Create new item
        /// </summary>
        /// <param name="createItemDto">Item creation data</param>
        /// <returns>Created item</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemDto createItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdItem = await _itemService.CreateItemAsync(createItemDto);
                return CreatedAtAction(nameof(GetItemById), new { id = createdItem.ItemId }, createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return StatusCode(500, new { message = "An error occurred while creating the item" });
            }
        }

        /// <summary>
        /// Update existing item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="updateItemDto">Item update data</param>
        /// <returns>Updated item</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ItemDto>> UpdateItem(Guid id, [FromBody] UpdateItemDto updateItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedItem = await _itemService.UpdateItemAsync(id, updateItemDto);
                if (updatedItem == null)
                    return NotFound(new { message = $"Item with ID {id} not found" });

                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item {ItemId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the item" });
            }
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            try
            {
                var result = await _itemService.DeleteItemAsync(id);
                if (!result)
                    return NotFound(new { message = $"Item with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item {ItemId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the item" });
            }
        }

        /// <summary>
        /// Get items by status
        /// </summary>
        /// <param name="status">Item status</param>
        /// <returns>List of items with specified status</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetItemsByStatus(string status)
        {
            try
            {
                var items = await _itemService.GetItemsByStatusAsync(status);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by status {Status}", status);
                return StatusCode(500, new { message = "An error occurred while retrieving items" });
            }
        }

        /// <summary>
        /// Get items by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of items for specified user</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetItemsByUserId(Guid userId)
        {
            try
            {
                var items = await _itemService.GetItemsByUserIdAsync(userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving items" });
            }
        }

        /// <summary>
        /// Get items by category ID
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of items in specified category</returns>
        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetItemsByCategory(Guid categoryId)
        {
            try
            {
                var items = await _itemService.GetItemsByCategoryIdAsync(categoryId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by category {CategoryId}", categoryId);
                return StatusCode(500, new { message = "An error occurred while retrieving items" });
            }
        }

        /// <summary>
        /// Get items by location ID
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <returns>List of items at specified location</returns>
        [HttpGet("location/{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetItemsByLocation(Guid locationId)
        {
            try
            {
                var items = await _itemService.GetItemsByLocationIdAsync(locationId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by location {LocationId}", locationId);
                return StatusCode(500, new { message = "An error occurred while retrieving items" });
            }
        }

        /// <summary>
        /// Search items by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching items</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> SearchItems([FromQuery] string searchTerm)
        {
            try
            {
                var items = await _itemService.SearchItemsAsync(searchTerm);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items with term {SearchTerm}", searchTerm);
                return StatusCode(500, new { message = "An error occurred while searching items" });
            }
        }

        /// <summary>
        /// Get items by date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of items within date range</returns>
        [HttpGet("date-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetItemsByDateRange(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new { message = "Start date must be before end date" });

                var items = await _itemService.GetItemsByDateRangeAsync(startDate, endDate);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by date range");
                return StatusCode(500, new { message = "An error occurred while retrieving items" });
            }
        }
    }
}
