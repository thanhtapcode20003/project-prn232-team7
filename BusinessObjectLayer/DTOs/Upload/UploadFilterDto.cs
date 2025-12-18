using System;

namespace BusinessObjectLayer.DTOs.Upload
{
    public class UploadFilterDto
    {
        public string? Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? StaffId { get; set; }
        public string? Type { get; set; }
        public string? SearchTerm { get; set; } // Search in item name
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

