using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.ServiceLocation;
using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/service-locations")]
    [ApiController]
    public class ServiceLocationController : ControllerBase
    {
        private readonly IServiceLocationService _serviceLocationService;

        public ServiceLocationController(IServiceLocationService serviceLocationService)
        {
            _serviceLocationService = serviceLocationService;
        }
        //[HttpGet("")]
        //public async Task<IActionResult> GetAllServiceLocations()
        //{
        //    var serviceLocations = await _serviceLocationService.GetAll();
        //    return Ok(ApiResponse<List<ServiceLocationResponse>>.Ok(serviceLocations, "Get all service locations successfully"));
        //}
        [HttpGet("{serviceLocationId}")]
        public async Task<IActionResult> GetServiceLocationById([FromRoute] Guid serviceLocationId)
        {
            var serviceLocation = await _serviceLocationService.GetById(serviceLocationId);
            return Ok(ApiResponse<ServiceLocationResponse>.Ok(serviceLocation, "Get service location by id successfully"));
        }
        [HttpGet("campus/{campusId}")]
        public async Task<IActionResult> GetServiceLocationsByCampusId([FromRoute] Guid campusId)
        {
            var serviceLocations = await _serviceLocationService.GetAllByCampusId(campusId);
            return Ok(ApiResponse<List<ServiceLocationResponse>>.Ok(serviceLocations, "Get service locations by campus id successfully"));



        }

        /// <summary>
        /// GET /api/service-locations/search - Search service locations with filters
        /// </summary>
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


        /// <summary>
        /// POST /api/service-locations - Create new service location
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ServiceLocationResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateServiceLocation([FromBody] ServiceLocationServiceRequest serviceLocation)
        {
            var newServiceLocation = await _serviceLocationService.Create(serviceLocation);
            return CreatedAtAction(
                nameof(GetServiceLocationById),
                new { id = newServiceLocation.ServiceLocationId },
                ApiResponse<ServiceLocationResponse>.Ok(newServiceLocation, "Service location created successfully")
            );
        }

        /// <summary>
        /// PUT /api/service-locations/{id} - Update existing service location
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ServiceLocationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateServiceLocation([FromRoute] Guid id, [FromBody] ServiceLocationServiceRequest serviceLocation)
        {
            var updatedServiceLocation = await _serviceLocationService.Update(id, serviceLocation);
            return Ok(ApiResponse<ServiceLocationResponse>.Ok(updatedServiceLocation, "Service location updated successfully"));
        }

        /// <summary>
        /// DELETE /api/service-locations/{id} - Delete service location
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteServiceLocation([FromRoute] Guid id)
        {
            var delete = await _serviceLocationService.Delete(id);
            return NoContent();
        }
    }
}
