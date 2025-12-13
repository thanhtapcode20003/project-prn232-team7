using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.Upload
{
    public class CreateUploadDto
    {
        [Required(ErrorMessage = "Item ID is required")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = null!;

        [StringLength(50, ErrorMessage = "StatusAccept cannot exceed 50 characters")]
        public string? StatusAccept { get; set; }
    }
}

