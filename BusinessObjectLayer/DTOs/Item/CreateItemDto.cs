using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.Item
{
    public class CreateItemDto
    {
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public string? FoundLocation { get; set; }

        public Guid? CurrentLocationId { get; set; }

        public string? Context { get; set; }

        public DateTime? FoundDate { get; set; }

        // ✅ BẮT BUỘC: FILE NẰM TRONG DTO
        [Required(ErrorMessage = "Image file is required")]
        public IFormFile File { get; set; } = null!;
    }
}
