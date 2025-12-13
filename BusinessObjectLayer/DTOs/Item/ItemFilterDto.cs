using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.DTOs.Item
{
    public class ItemFilterDto
    {
        public string? Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? LocationId { get; set; }
        public string? SearchTerm { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
