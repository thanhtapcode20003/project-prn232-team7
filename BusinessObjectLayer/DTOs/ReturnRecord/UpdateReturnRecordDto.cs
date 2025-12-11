using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class UpdateReturnRecordDto
    {
        [Required(ErrorMessage = "Item ID is required")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Found User ID is required")]
        public Guid FoundUserId { get; set; }

        public Guid? ReceiverUserId { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string Status { get; set; } = null!;
    }
}

