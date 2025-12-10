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
        /// Get all items (without filters)
        /// </summary>
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
        /// Search/Filter items with multiple optional parameters
        /// </summary>
        /// <param name="status">Filter by status</param>
        /// <param name="userId">Filter by user ID</param>
        /// <param name="categoryId">Filter by category ID</param>
        /// <param name="locationId">Filter by location ID</param>
        /// <param name="searchTerm">Search in item name and description</param>
        /// <param name="fromDate">Filter from date (YYYY-MM-DD)</param>
        /// <param name="toDate">Filter to date (YYYY-MM-DD)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated list of filtered items</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<ItemDto>>> SearchItems(
            [FromQuery] string? status = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] Guid? locationId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new ItemFilterDto
                {
                    Status = status,
                    UserId = userId,
                    CategoryId = categoryId,
                    LocationId = locationId,
                    SearchTerm = searchTerm,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _itemService.SearchItemsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items");
                return StatusCode(500, new { message = "An error occurred while searching items" });
            }
        }

        /// <summary>
        /// Get item by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Create new item
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Update existing item
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Delete item
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
    }
}
