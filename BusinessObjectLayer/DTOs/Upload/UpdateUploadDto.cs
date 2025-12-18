using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.Upload
{
    public class UpdateUploadDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public Guid CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "Lost location cannot exceed 500 characters")]
        public string? LostLocation { get; set; }

        public DateTime? LostDate { get; set; }

        public string? Content { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string? Status { get; set; }
        [Required(ErrorMessage = "Campus ID is required")]
        public Guid CampusId { get; set; }

        //public Guid? Staffid { get; set; }

        //[StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
        //public string? Type { get; set; }

        //public string? Note { get; set; }
    }
}

