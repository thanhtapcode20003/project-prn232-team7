using System;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class ReturnRecordDto
    {
        public Guid ReturnId { get; set; }
        public Guid ItemId { get; set; }
        public Guid FoundUserId { get; set; }
        public Guid? ReceiverUserId { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; } = null!;

        // Navigation properties
        public string? ItemName { get; set; }
        public string? FoundUserName { get; set; }
        public string? ReceiverUserName { get; set; }
    }
}


