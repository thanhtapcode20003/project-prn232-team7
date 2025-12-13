using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.Upload
{
    public class UpdateUploadDto
    {
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string? Status { get; set; }

        [StringLength(50, ErrorMessage = "StatusAccept cannot exceed 50 characters")]
        public string? StatusAccept { get; set; }
    }
}

