using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class UpdateReturnRecordDto
    {
        [Required(ErrorMessage = "Item ID is required")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Staff ID is required")]
        public Guid StaffId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        public string? ImgCccdFont { get; set; }
        
        public string? ImgCccdBack { get; set; }
        
        public string? EvidenceImg { get; set; }
        
        public string? ConfirmImg { get; set; }
        
        public string? VerifyNotes { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = null!;
    }
}




