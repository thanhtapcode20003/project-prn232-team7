using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.ServiceLocation;
using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/serviceLocation")]
    [ApiController]
    public class ServiceLocationController : ControllerBase
    {
        private readonly IServiceLocationService _serviceLocationService;
        public ServiceLocationController(IServiceLocationService serviceLocationService)
        {
            _serviceLocationService = serviceLocationService;

        }
        [HttpGet("")]
        public async Task<IActionResult> GetAllServiceLocations()
        {
            var serviceLocations = await _serviceLocationService.GetAll();
            return Ok(ApiResponse<List<ServiceLocation>>.Ok(serviceLocations, "Get all service locations successfully"));
        }
        [HttpGet("{serviceLocationId}")]
        public async Task<IActionResult> GetServiceLocationById([FromRoute] Guid serviceLocationId)
        {
            var serviceLocation = await _serviceLocationService.GetById(serviceLocationId);
            return Ok(ApiResponse<ServiceLocation>.Ok(serviceLocation, "Get service location by id successfully"));
        }
        [HttpGet("campus/{campusId}")]
        public async Task<IActionResult> GetServiceLocationsByCampusId([FromRoute] Guid campusId)
        {
            var serviceLocations = await _serviceLocationService.GetAllByCampusId(campusId);
            return Ok(ApiResponse<List<ServiceLocation>>.Ok(serviceLocations, "Get service locations by campus id successfully"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchServiceLocations(
            [FromQuery] string? name = null,
            [FromQuery] string? campusName = null,
            [FromQuery] string? address = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new ServicelocationFilter()
            {
                Name = name,
                CampusName = campusName,
                Address = address,
                Page = page,
                PageSize = pageSize
            };
            var result = await _serviceLocationService.SearchServiceLocationsAsync(filter);
            return Ok(ApiResponse<PaginationResult<List<ServiceLocationResponse>>>.Ok(
                result,
                "Search service locations successfully"
            ));
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateServiceLocation([FromBody] ServiceLocationServiceRequest serviceLocation)
        {
            var newServiceLocation = await _serviceLocationService.Create(serviceLocation);
            return Ok(ApiResponse<ServiceLocation>.Ok(newServiceLocation, "Service location created successfully"));
        }
        [HttpPost("delete")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteServiceLocation([FromBody] Guid serviceLocationId)
        {
            var delete = await _serviceLocationService.Delete(serviceLocationId);
            return Ok(ApiResponse<bool>.Ok(delete, "Service location deleted successfully"));
        }
        [HttpPut("update/{serviceLocationId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateServiceLocation([FromRoute] Guid serviceLocationId, [FromBody] ServiceLocationServiceRequest serviceLocation)
        {
            var updatedServiceLocation = await _serviceLocationService.Update(serviceLocationId, serviceLocation);
            return Ok(ApiResponse<ServiceLocation>.Ok(updatedServiceLocation, "Service location updated successfully"));
        }

    }

}
