using System;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class ReturnRecordFilterDto
    {
        public string? Status { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? FoundUserId { get; set; }
        public Guid? ReceiverUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

