using System;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class ReturnRecordFilterDto
    {
        public string? Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid? StaffId { get; set; }
        public Guid? ItemId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
