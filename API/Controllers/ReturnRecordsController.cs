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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ReturnRecordDto>>> GetById(Guid id)
        {
            try
            {
                var record = await _returnRecordService.GetByIdAsync(id);
                if (record == null)
                    return NotFound(new { message = $"Return record with ID {id} not found" });

                return Ok(ApiResponse<ReturnRecordDto>.Ok(record, "Return record retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting return record {ReturnRecordId}", id);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(typeof(ApiResponse<ReturnRecordDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ReturnRecordDto>>> Create([FromForm] CreateReturnRecordDto dto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating return record");
                return StatusCode(500, new { message = "An error occurred while creating return record" });
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ReturnRecordDto>>> Update(Guid id, [FromForm] UpdateReturnRecordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _returnRecordService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Return record with ID {id} not found" });

                return Ok(ApiResponse<ReturnRecordDto>.Ok(updated, "Return record updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating return record {ReturnRecordId}", id);
                return StatusCode(500, new { message = "An error occurred while updating return record" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _returnRecordService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Return record with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting return record {ReturnRecordId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting return record" });
            }
        }
    }
}
