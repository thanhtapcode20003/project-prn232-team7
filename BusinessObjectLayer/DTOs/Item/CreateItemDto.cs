using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.Item
{
    public class CreateItemDto
    {
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200, ErrorMessage = "Item name cannot exceed 200 characters")]
        public string Name { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public string? Img { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string? Status { get; set; }


        public string? FoundLocation { get; set; }

        public Guid? CurrentLocationId { get; set; }

        public string? Context { get; set; }

        public DateTime? FoundDate { get; set; }
    }
}
