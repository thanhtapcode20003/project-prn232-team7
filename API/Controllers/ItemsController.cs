using BusinessObjectLayer.DTOs.Item;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/items")]
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
        /// <param name="fromDate">Filter from date (YYYY-MM-DD HH:mm:ss)</param>
        /// <param name="toDate">Filter to date (YYYY-MM-DD HH:mm:ss)</param>
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
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new ItemFilterDto
                {

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

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ItemDto>>> CreateItem(
             [FromForm] CreateItemDto createItemDto, IFormFile file)
        {


            var createdItem = await _itemService.CreateItemAsync(createItemDto, file);

            return CreatedAtAction(
                nameof(GetItemById),
                new { id = createdItem.Id },
                ApiResponse<ItemDto>.Ok(createdItem, "Item created successfully")
            );
        }


        /// <summary>
        /// Update existing item
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
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
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
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
