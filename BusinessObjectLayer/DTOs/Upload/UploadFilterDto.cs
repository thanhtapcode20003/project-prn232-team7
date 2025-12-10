using System;

namespace BusinessObjectLayer.DTOs.Upload
{
    public class UploadFilterDto
    {
        public string? Status { get; set; }
        public string? StatusAccept { get; set; }
        public Guid? ItemId { get; set; }
        public string? SearchTerm { get; set; } // Search in item name
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

