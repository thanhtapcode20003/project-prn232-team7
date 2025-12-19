using BusinessObjectLayer.DTOs.ReturnRecord;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/return-records")]
    public class ReturnRecordsController : ControllerBase
    {
        private readonly IReturnRecordService _returnRecordService;
        private readonly ILogger<ReturnRecordsController> _logger;

        public ReturnRecordsController(IReturnRecordService returnRecordService, ILogger<ReturnRecordsController> logger)
        {
            _returnRecordService = returnRecordService;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/return-records - Get all return records with filters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<BusinessObjectLayer.IService.PagedResult<ReturnRecordDto>>>> GetReturnRecords(
            [FromQuery] string? status = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] Guid? staffId = null,
            [FromQuery] Guid? itemId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new ReturnRecordFilterDto
                {
                    Status = status,
                    UserId = userId,
                    StaffId = staffId,
                    ItemId = itemId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _returnRecordService.SearchAsync(filter);
                return Ok(ApiResponse<BusinessObjectLayer.IService.PagedResult<ReturnRecordDto>>.Ok(result, "Return records retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting return records");
                return StatusCode(500, new { message = "An error occurred while retrieving return records" });
            }
        }

        /// <summary>
        /// GET /api/return-records/{id} - Get return record by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ReturnRecordDto>>> GetById(Guid id)
        {
            var record = await _returnRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound(new { message = $"Return record with ID {id} not found" });

            return Ok(ApiResponse<ReturnRecordDto>.Ok(record, "Return record retrieved successfully"));
        }

        /// <summary>
        /// POST /api/return-records - Create new return record
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(typeof(ApiResponse<ReturnRecordDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ReturnRecordDto>>> Create([FromForm] CreateReturnRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _returnRecordService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<ReturnRecordDto>.Ok(created, "Return record created successfully")
            );
        }

        /// <summary>
        /// PUT /api/return-records/{id} - Update existing return record
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<ReturnRecordDto>>> Update(Guid id, [FromForm] UpdateReturnRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _returnRecordService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Return record with ID {id} not found" });

            return Ok(ApiResponse<ReturnRecordDto>.Ok(updated, "Return record updated successfully"));
        }

        /// <summary>
        /// DELETE /api/return-records/{id} - Delete return record
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _returnRecordService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Return record with ID {id} not found" });

            return NoContent();
        }
    }
}
