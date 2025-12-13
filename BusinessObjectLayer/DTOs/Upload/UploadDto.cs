using System;

namespace BusinessObjectLayer.DTOs.Upload
{
    public class UploadDto
    {
        public Guid UploadId { get; set; }
        public Guid ItemId { get; set; }
        public string FileUrl { get; set; } = null!;
        public DateTime? UploadTime { get; set; }
        public string Status { get; set; } = null!;
        public string? StatusAccept { get; set; }
        
        // Navigation properties
        public string? ItemName { get; set; }
    }
}

