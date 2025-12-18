using System;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class ReturnRecordDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid StaffId { get; set; }
        public Guid UserId { get; set; }
        public string? ImgCccdFont { get; set; }
        public string? ImgCccdBack { get; set; }
        public string? EvidenceImg { get; set; }
        public string? ConfirmImg { get; set; }
        public string? VerifyNotes { get; set; }
        public string? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdate { get; set; }

        // Navigation properties
        public string? ItemName { get; set; }
        public string? StaffName { get; set; }
        public string? UserName { get; set; }
    }
}




