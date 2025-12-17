
using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/campus")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCampuses()
        {
            var campuses = await _campusService.GetAllCampuses();
            return Ok(ApiResponse<List<CampusResponse>>.Ok(campuses, "Get all campuses successfully"));
        }

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


        [HttpGet("{campusId}")]
        public async Task<IActionResult> GetCampusById([FromRoute] Guid campusId)
        {
            var campus = await _campusService.GetCampusById(campusId);
            return Ok(ApiResponse<CampusResponse>.Ok(campus, "Get campus by id successfully"));
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCampus([FromBody] CampusRequest campus)
        {
            var newCampus = await _campusService.CreateCampus(campus);
            return Ok(ApiResponse<CampusResponse>.Ok(newCampus, "Campus created successfully"));
        }

        [HttpDelete("delete")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCampus([FromBody] Guid campusId)
        {
            var delete = await _campusService.DeleteCampus(campusId);
            return Ok(ApiResponse<bool>.Ok(delete, "Campus deleted successfully"));
        }

        [HttpPut("update/{campusId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCampus([FromRoute] Guid campusId, [FromBody] CampusRequest campus)
        {
            var updatedCampus = await _campusService.UpdateCampus(campusId, campus);
            return Ok(ApiResponse<CampusResponse>.Ok(updatedCampus, "Campus updated successfully"));
        }


    }
}