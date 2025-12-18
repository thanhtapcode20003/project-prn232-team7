using BusinessObjectLayer.DTOs.Upload;
using Microsoft.AspNetCore.Http;

namespace BusinessObjectLayer.IService
{
    public interface IUploadService
    {
        Task<List<UploadDto>> GetAllUploadsAsync();
        Task<UploadDto?> GetUploadByIdAsync(Guid uploadId);
        Task<List<UploadDto>> GetUploadsByCategoryIdAsync(Guid categoryId);
        Task<UploadDto> UploadFileAsync(CreateUploadDto createUploadDto, IFormFile file);
        Task<UploadDto?> UpdateUploadAsync(Guid uploadId, UpdateUploadDto updateUploadDto);
        Task<bool> DeleteUploadAsync(Guid uploadId);
        Task<bool> DeleteFileAsync(string fileUrl);

        // Search and pagination
        Task<PagedResult<UploadDto>> SearchUploadsAsync(UploadFilterDto filter);

        Task<UploadDto> SendNotificationUpload(Guid uploadId, SendNotificationDTO sendNotificationDTO);
        Task<UploadDto> UpdateSendNotificationUpload(Guid uploadId, SendNotificationDTO sendNotificationDTO);
    }
}

