using Microsoft.AspNetCore.Http;
using System;

namespace API.DTOs
{
    public class FileUploadDto
    {
        public Guid ItemId { get; set; }
        public IFormFile File { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public string? StatusAccept { get; set; }
    }
}

