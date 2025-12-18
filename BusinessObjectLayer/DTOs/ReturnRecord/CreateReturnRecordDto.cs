using System;
using Microsoft.AspNetCore.Http;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class CreateReturnRecordDto
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }

        // Các file ảnh upload (optional)
        public IFormFile? ImgCccdFont { get; set; }
        public IFormFile? ImgCccdBack { get; set; }
        public IFormFile? EvidenceImg { get; set; }
        public IFormFile? ConfirmImg { get; set; }

        public string? VerifyNotes { get; set; }
        public string? Status { get; set; }
    }
}
