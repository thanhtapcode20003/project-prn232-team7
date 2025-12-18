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
        [HttpGet("")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoriesService.GetAllCate();
            return Ok(ApiResponse<List<CategoriesResponse>>.Ok(categories, "Get all categories successfully"));
        }
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId)
        {
            var category = await _categoriesService.GetCateById(categoryId);
            return Ok(ApiResponse<CategoriesResponse>.Ok(category, "Get category by id successfully"));


        }
        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoriesRequest category)
        {
            var createdCategory = await _categoriesService.CreateCate(category);
            return Ok(ApiResponse<CategoriesResponse>.Ok(createdCategory, "Category created successfully"));
        }
        [HttpDelete("delete")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCategory([FromQuery] Guid categoryId)
        {
            var result = await _categoriesService.DeleteCate(categoryId);
            return Ok(ApiResponse<bool>.Ok(result, "Category deleted successfully"));
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            //[FromQuery] string? description = null,
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
    }
}
