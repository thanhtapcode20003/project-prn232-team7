using BusinessObjectLayer.DTOs.ReturnRecord;
using BusinessObjectLayer.IService;
using BusinessObjectLayer.Exceptions;
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
        public async Task<ActionResult<PagedResult<ReturnRecordDto>>> GetAllReturnRecords(
            [FromQuery] string? status = null,
            [FromQuery] Guid? itemId = null,
            [FromQuery] Guid? staffId = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new ReturnRecordFilterDto
                {
                    Status = status,
                    ItemId = itemId,
                    StaffId = staffId,
                    UserId = userId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SearchTerm = searchTerm,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _returnRecordService.SearchReturnRecordsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all return records");
                return StatusCode(500, new { message = "An error occurred while retrieving return records" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReturnRecordDto>> GetReturnRecordById(Guid id)
        {
            try
            {
                var returnRecord = await _returnRecordService.GetReturnRecordByIdAsync(id);
                if (returnRecord == null)
                    return NotFound(new { message = $"Return record with ID {id} not found" });

                return Ok(returnRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting return record {ReturnRecordId}", id);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReturnRecordDto>> CreateReturnRecord([FromBody] CreateReturnRecordDto createReturnRecordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdReturnRecord = await _returnRecordService.CreateReturnRecordAsync(createReturnRecordDto);
                return CreatedAtAction(nameof(GetReturnRecordById), new { id = createdReturnRecord.Id }, createdReturnRecord);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found when creating return record");
                return NotFound(new { message = ex.Error.Message, details = ex.Error.Details });
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "API error creating return record");
                return StatusCode(ex.Error.StatusCode, new { message = ex.Error.Message, details = ex.Error.Details });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating return record");
                return StatusCode(500, new { message = "An error occurred while creating return record" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReturnRecordDto>> UpdateReturnRecord(Guid id, [FromBody] UpdateReturnRecordDto updateReturnRecordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedReturnRecord = await _returnRecordService.UpdateReturnRecordAsync(id, updateReturnRecordDto);
                if (updatedReturnRecord == null)
                    return NotFound(new { message = $"Return record with ID {id} not found" });

                return Ok(updatedReturnRecord);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found when updating return record {ReturnRecordId}", id);
                return NotFound(new { message = ex.Error.Message, details = ex.Error.Details });
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "API error updating return record {ReturnRecordId}", id);
                return StatusCode(ex.Error.StatusCode, new { message = ex.Error.Message, details = ex.Error.Details });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating return record {ReturnRecordId}", id);
                return StatusCode(500, new { message = "An error occurred while updating return record" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReturnRecord(Guid id)
        {
            try
            {
                var result = await _returnRecordService.DeleteReturnRecordAsync(id);
                if (!result)
                    return NotFound(new { message = $"Return record with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting return record {ReturnRecordId}", id);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
    }
}




