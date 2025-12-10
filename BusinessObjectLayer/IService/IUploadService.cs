using BusinessObjectLayer.DTOs.Upload;
using Microsoft.AspNetCore.Http;

namespace BusinessObjectLayer.IService
{
    public interface IUploadService
    {
        Task<List<UploadDto>> GetAllUploadsAsync();
        Task<UploadDto?> GetUploadByIdAsync(Guid uploadId);
        Task<List<UploadDto>> GetUploadsByItemIdAsync(Guid itemId);
        Task<UploadDto> UploadFileAsync(Guid itemId, IFormFile file, string status = "Pending", string? statusAccept = null);
        Task<UploadDto?> UpdateUploadAsync(Guid uploadId, UpdateUploadDto updateUploadDto);
        Task<bool> DeleteUploadAsync(Guid uploadId);
        Task<bool> DeleteFileAsync(string fileUrl);
    }
}

