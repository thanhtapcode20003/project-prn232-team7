using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.DTOs.Item
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Img { get; set; }
        public Guid CategoryId { get; set; }
        public string? Status { get; set; }
        public DateTime? Date { get; set; }
        public string? FoundLocation { get; set; }
        public Guid? CurrentLocationId { get; set; }
        public string? Context { get; set; }
        public Guid UserId { get; set; }
        public DateTime? FoundDate { get; set; }

        // Navigation properties
        public string? CategoryName { get; set; }
        public string? CurrentLocationName { get; set; }
        public string? UserName { get; set; }
    }
}
