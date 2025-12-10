using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.DTOs.Item
{
    public class CreateItemDto
    {
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200, ErrorMessage = "Item name cannot exceed 200 characters")]
        public string ItemName { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public DateOnly? LostDate { get; set; }

        public TimeOnly? LostTime { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "User is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public Guid LocationId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = null!;
    }
}
