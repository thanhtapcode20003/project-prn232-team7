using System;
using Microsoft.AspNetCore.Http;

namespace BusinessObjectLayer.DTOs.ReturnRecord
{
    public class UpdateReturnRecordDto
    {
        // Nếu gửi file mới -> cập nhật, nếu null -> giữ nguyên
        public IFormFile? ImgCccdFont { get; set; }
        public IFormFile? ImgCccdBack { get; set; }
        public IFormFile? EvidenceImg { get; set; }
        public IFormFile? ConfirmImg { get; set; }

        public string? VerifyNotes { get; set; }
        public string? Status { get; set; }
    }
}
