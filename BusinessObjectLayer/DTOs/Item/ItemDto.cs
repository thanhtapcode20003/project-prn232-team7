using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.DTOs.Item
{
    public class ItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Description { get; set; }
        public DateOnly? LostDate { get; set; }
        public TimeOnly? LostTime { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        public Guid LocationId { get; set; }
        public string Status { get; set; } = null!;

        // Navigation properties
        public string? CategoryName { get; set; }
        public string? LocationName { get; set; }
        public string? UserName { get; set; }
    }
}
