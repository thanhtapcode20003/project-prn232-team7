using BusinessObjectLayer.DTOs.Upload;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Repository;
using PagedResult = BusinessObjectLayer.IService.PagedResult<BusinessObjectLayer.DTOs.Upload.UploadDto>;

namespace BusinessObjectLayer.Services
{
    public class UploadService : IUploadService
    {
        private readonly UploadRepository _uploadRepository;
        private readonly ItemRepository _itemRepository;
        private readonly IWebHostEnvironment _environment;
        private const string UploadFolder = "uploads";
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };

        public UploadService(UploadRepository uploadRepository, ItemRepository itemRepository, IWebHostEnvironment environment)
        {
            _uploadRepository = uploadRepository;
            _itemRepository = itemRepository;
            _environment = environment;
        }

        public async Task<List<UploadDto>> GetAllUploadsAsync()
        {
            var uploads = await _uploadRepository.GetAllWithItemAsync();
            return uploads.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<UploadDto>> SearchUploadsAsync(UploadFilterDto filter)
        {
            var uploads = await _uploadRepository.SearchUploadsAsync(
                status: filter.Status,
                statusAccept: filter.StatusAccept,
                itemId: filter.ItemId,
                searchTerm: filter.SearchTerm,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _uploadRepository.CountUploadsAsync(
                status: filter.Status,
                statusAccept: filter.StatusAccept,
                itemId: filter.ItemId,
                searchTerm: filter.SearchTerm,
                fromDate: filter.FromDate,
                toDate: filter.ToDate
            );

            return new PagedResult
            {
                Items = uploads.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<UploadDto?> GetUploadByIdAsync(Guid uploadId)
        {
            var upload = await _uploadRepository.GetByIdWithItemAsync(uploadId);
            return upload == null ? null : MapToDto(upload);
        }

        public async Task<List<UploadDto>> GetUploadsByItemIdAsync(Guid itemId)
        {
            var uploads = await _uploadRepository.GetByItemIdAsync(itemId);
            return uploads.Select(MapToDto).ToList();
        }

        public async Task<UploadDto> UploadFileAsync(Guid itemId, IFormFile file, string status = "Pending", string? statusAccept = null)
        {
            // Validate item exists
            if (!await _uploadRepository.ItemExistsAsync(itemId))
            {
                throw new InvalidOperationException($"Item with ID {itemId} does not exist");
            }

            // Validate file
            ValidateFile(file);

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate file URL
            var fileUrl = $"/{UploadFolder}/{fileName}";

            // Create upload record
            var upload = new Upload
            {
                UploadId = Guid.NewGuid(),
                ItemId = itemId,
                FileUrl = fileUrl,
                UploadTime = DateTime.UtcNow,
                Status = status,
                StatusAccept = statusAccept
            };

            await _uploadRepository.CreateAsync(upload);
            var createdUpload = await _uploadRepository.GetByIdWithItemAsync(upload.UploadId);
            return MapToDto(createdUpload!);
        }

        public async Task<UploadDto?> UpdateUploadAsync(Guid uploadId, UpdateUploadDto updateUploadDto)
        {
            var existingUpload = await _uploadRepository.GetByIdAsync(uploadId);
            if (existingUpload == null)
                return null;

            if (!string.IsNullOrEmpty(updateUploadDto.Status))
                existingUpload.Status = updateUploadDto.Status;

            if (updateUploadDto.StatusAccept != null)
                existingUpload.StatusAccept = updateUploadDto.StatusAccept;

            await _uploadRepository.UpdateAsync(existingUpload);
            var updatedUpload = await _uploadRepository.GetByIdWithItemAsync(uploadId);
            return MapToDto(updatedUpload!);
        }

        public async Task<bool> DeleteUploadAsync(Guid uploadId)
        {
            var upload = await _uploadRepository.GetByIdAsync(uploadId);
            if (upload == null)
                return false;

            // Delete physical file
            await DeleteFileAsync(upload.FileUrl);

            // Delete database record
            await _uploadRepository.RemoveAsync(upload);
            return true;
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                var filePath = fileUrl.StartsWith("/") 
                    ? fileUrl.Substring(1) 
                    : fileUrl;
                
                var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, filePath);
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required and cannot be empty");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }
        }

        private UploadDto MapToDto(Upload upload)
        {
            return new UploadDto
            {
                UploadId = upload.UploadId,
                ItemId = upload.ItemId,
                FileUrl = upload.FileUrl,
                UploadTime = upload.UploadTime,
                Status = upload.Status,
                StatusAccept = upload.StatusAccept,
                ItemName = upload.Item?.ItemName
            };
        }
    }
}

