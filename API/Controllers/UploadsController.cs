using BusinessObjectLayer.DTOs.Upload;
using BusinessObjectLayer.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly ILogger<UploadsController> _logger;

        public UploadsController(IUploadService uploadService, ILogger<UploadsController> logger)
        {
            _uploadService = uploadService;
            _logger = logger;
        }

        /// <summary>
        /// Get all uploads
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UploadDto>>> GetAllUploads()
        {
            try
            {
                var uploads = await _uploadService.GetAllUploadsAsync();
                return Ok(uploads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all uploads");
                return StatusCode(500, new { message = "An error occurred while retrieving uploads" });
            }
        }

        /// <summary>
        /// Get upload by ID
        /// </summary>
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
        /// Get uploads by Item ID
        /// </summary>
        [HttpGet("item/{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UploadDto>>> GetUploadsByItemId(Guid itemId)
        {
            try
            {
                var uploads = await _uploadService.GetUploadsByItemIdAsync(itemId);
                return Ok(uploads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting uploads for item {ItemId}", itemId);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Upload a file for an item
        /// </summary>
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UploadDto>> UploadFile(
            [FromForm] Guid itemId,
            [FromForm] IFormFile file,
            [FromForm] string status = "Pending",
            [FromForm] string? statusAccept = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "File is required" });
                }

                var upload = await _uploadService.UploadFileAsync(itemId, file, status, statusAccept);
                return CreatedAtAction(nameof(GetUploadById), new { id = upload.UploadId }, upload);
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

        /// <summary>
        /// Update upload information
        /// </summary>
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

        /// <summary>
        /// Delete upload
        /// </summary>
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
    }
}

