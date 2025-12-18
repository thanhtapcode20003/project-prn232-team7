using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/campuses")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        /// <summary>
        /// GET /api/campuses - Get all campuses
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCampuses()
        {
            var campuses = await _campusService.GetAllCampuses();
            return Ok(ApiResponse<List<CampusResponse>>.Ok(campuses, "Get all campuses successfully"));
        }

        /// <summary>
        /// GET /api/campuses/search - Search campuses with filters
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCampuses(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new CampusFilterDto
            {
                Name = name,
                Page = page,
                PageSize = pageSize
            };

            var result = await _campusService.SearchCampuses(filter);

            return Ok(ApiResponse<PaginationResult<List<CampusResponse>>>.Ok(
                result,
                "Search campuses successfully"
            ));
        }

        /// <summary>
        /// GET /api/campuses/{id} - Get campus by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampusById([FromRoute] Guid id)
        {
            var campus = await _campusService.GetCampusById(id);
            return Ok(ApiResponse<CampusResponse>.Ok(campus, "Get campus by id successfully"));
        }

        /// <summary>
        /// POST /api/campuses - Create new campus
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CampusResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCampus([FromBody] CampusRequest campus)
        {
            var newCampus = await _campusService.CreateCampus(campus);
            return CreatedAtAction(
                nameof(GetCampusById),
                new { id = newCampus.CampusId },
                ApiResponse<CampusResponse>.Ok(newCampus, "Campus created successfully")
            );
        }

        /// <summary>
        /// PUT /api/campuses/{id} - Update existing campus
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CampusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCampus([FromRoute] Guid id, [FromBody] CampusRequest campus)
        {
            var updatedCampus = await _campusService.UpdateCampus(id, campus);
            return Ok(ApiResponse<CampusResponse>.Ok(updatedCampus, "Campus updated successfully"));
        }

        /// <summary>
        /// DELETE /api/campuses/{id} - Delete campus
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCampus([FromRoute] Guid id)
        {
            var delete = await _campusService.DeleteCampus(id);
            return NoContent();
        }
    }
}