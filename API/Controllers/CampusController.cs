using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using BusinessObjectLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/campus")]
    [ApiController]
    public class CampusController : ControllerBase
    {

        private readonly CampusService _campusService;
        public CampusController(CampusService campusService)
        {
            _campusService = campusService;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAllCampuses()
        {
            var campuses = await _campusService.GetAllCampuses();
            return Ok(ApiResponse<List<DataAccessLayer.Models.Campus>>.Ok(campuses, "Get all campuses successfully"));
        }
        [HttpGet("{campusId}")]
        public async Task<IActionResult> GetCampusById([FromRoute] Guid campusId)
        {
            var campus = await _campusService.GetCampusById(campusId);
            return Ok(ApiResponse<DataAccessLayer.Models.Campus>.Ok(campus, "Get campus by id successfully"));
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCampus([FromBody] CampusRequest campus)
        {
            var newCampus = await _campusService.CreateCampus(campus);
            return Ok(ApiResponse<DataAccessLayer.Models.Campus>.Ok(newCampus, "Campus created successfully"));
        }
        [HttpPost("delete")]
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
            return Ok(ApiResponse<DataAccessLayer.Models.Campus>.Ok(updatedCampus, "Campus updated successfully"));
        }

    }
}
