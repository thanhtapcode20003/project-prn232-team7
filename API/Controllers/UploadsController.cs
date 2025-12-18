using BusinessObjectLayer.DTOs.Upload;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly ILogger<UploadsController> _logger;

        public UploadsController(IUploadService uploadService, ILogger<UploadsController> logger)
        {
            _uploadService = uploadService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BusinessObjectLayer.IService.PagedResult<UploadDto>>> GetAllUploads(
            [FromQuery] string? status = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] Guid? staffId = null,
            [FromQuery] string? type = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new UploadFilterDto
                {
                    Status = status,
                    UserId = userId,
                    CategoryId = categoryId,
                    StaffId = staffId,
                    Type = type,
                    SearchTerm = searchTerm,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _uploadService.SearchUploadsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all uploads");
                return StatusCode(500, new { message = "An error occurred while retrieving uploads" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UploadDto>> GetUploadById(Guid id)
        {
            try
            {
                var upload = await _uploadService.GetUploadByIdAsync(id);
                if (upload == null)
                    return NotFound(new { message = $"Upload with ID {id} not found" });

                return Ok(upload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upload {UploadId}", id);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// GET /api/uploads/category/{categoryId} - Get uploads by category
        /// Alternative: Use query parameter GET /api/uploads?categoryId={id}
        /// </summary>
        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UploadDto>>> GetUploadsByCategoryId(Guid categoryId)
        {
            try
            {
                var uploads = await _uploadService.GetUploadsByCategoryIdAsync(categoryId);
                return Ok(uploads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting uploads for category {CategoryId}", categoryId);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UploadDto>> UploadFile([FromForm] CreateUploadDto createUploadDto, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "File is required" });
                }

                var upload = await _uploadService.UploadFileAsync(createUploadDto, file);

                return CreatedAtAction(nameof(GetUploadById), new { id = upload.Id }, upload);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid file upload request");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, new { message = "An error occurred while uploading the file" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UploadDto>> UpdateUpload(Guid id, [FromBody] UpdateUploadDto updateUploadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedUpload = await _uploadService.UpdateUploadAsync(id, updateUploadDto);
                if (updatedUpload == null)
                    return NotFound(new { message = $"Upload with ID {id} not found" });

                return Ok(updatedUpload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating upload {UploadId}", id);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUpload(Guid id)
        {
            try
            {
                var result = await _uploadService.DeleteUploadAsync(id);
                if (!result)
                    return NotFound(new { message = $"Upload with ID {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting upload {UploadId}", id);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// PUT /api/uploads/{id}/notification - Add or update notification for upload
        /// RESTful: Use PUT to create or update the notification sub-resource
        /// </summary>
        [HttpPut("{id}/notification")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertNotification(
            [FromRoute] Guid id,
            [FromBody] SendNotificationDTO send)
        {
            try
            {
                // Try to update first, if not exists then create
                var result = await _uploadService.UpdateSendNotificationUpload(id, send);
                
                if (result == null)
                {
                    // If update returns null, try to create
                    result = await _uploadService.SendNotificationUpload(id, send);
                }

                if (result == null)
                    return NotFound(new { message = "Upload not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PATCH /api/uploads/{id}/notification - Partially update notification
        /// </summary>
        [HttpPatch("{id}/notification")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Staff))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchNotification(
            [FromRoute] Guid id,
            [FromBody] SendNotificationDTO send)
        {
            try
            {
                var result = await _uploadService.UpdateSendNotificationUpload(id, send);

                if (result == null)
                    return NotFound(new { message = "Upload not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

