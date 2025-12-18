using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Categories;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
        
        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        /// <summary>
        /// GET /api/categories - Get all categories
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoriesService.GetAllCate();
            return Ok(ApiResponse<List<CategoriesResponse>>.Ok(categories, "Get all categories successfully"));
        }

        /// <summary>
        /// GET /api/categories/search - Search categories with filters
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new CategoriesFillter
            {
                Name = name,
                Page = page,
                PageSize = pageSize
            };
            
            var result = await _categoriesService.GetCatesPaged(filter);
            return Ok(ApiResponse<PaginationResult<List<CategoriesResponse>>>.Ok(
                result,
                "Search categories successfully"
            ));
        }

        /// <summary>
        /// GET /api/categories/{id} - Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var category = await _categoriesService.GetCateById(id);
            return Ok(ApiResponse<CategoriesResponse>.Ok(category, "Get category by id successfully"));
        }

        /// <summary>
        /// POST /api/categories - Create new category
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CategoriesResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoriesRequest category)
        {
            var createdCategory = await _categoriesService.CreateCate(category);
            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = createdCategory.Id },
                ApiResponse<CategoriesResponse>.Ok(createdCategory, "Category created successfully")
            );
        }

        /// <summary>
        /// PUT /api/categories/{id} - Update existing category
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CategoriesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] CategoriesRequest category)
        {
            // Note: Need to implement UpdateCate method in service
            // var updatedCategory = await _categoriesService.UpdateCate(id, category);
            // return Ok(ApiResponse<CategoriesResponse>.Ok(updatedCategory, "Category updated successfully"));
            
            return StatusCode(501, new { message = "Update category not implemented yet" });
        }

        /// <summary>
        /// DELETE /api/categories/{id} - Delete category
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var result = await _categoriesService.DeleteCate(id);
            return NoContent();
        }
    }
}
