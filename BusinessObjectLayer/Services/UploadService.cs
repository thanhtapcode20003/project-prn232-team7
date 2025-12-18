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
        private readonly IWebHostEnvironment _environment;
        private const string UploadFolder = "uploads";
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };

        public UploadService(UploadRepository uploadRepository, IWebHostEnvironment environment)
        {
            _uploadRepository = uploadRepository;
            _environment = environment;
        }

        public async Task<List<UploadDto>> GetAllUploadsAsync()
        {
            var uploads = await _uploadRepository.GetAllWithDetailsAsync();
            return uploads.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<UploadDto>> SearchUploadsAsync(UploadFilterDto filter)
        {
            var uploads = await _uploadRepository.SearchUploadsAsync(
                status: filter.Status,
                userId: filter.UserId,
                categoryId: filter.CategoryId,
                staffId: filter.StaffId,
                type: filter.Type,
                searchTerm: filter.SearchTerm,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _uploadRepository.CountUploadsAsync(
                status: filter.Status,
                userId: filter.UserId,
                categoryId: filter.CategoryId,
                staffId: filter.StaffId,
                type: filter.Type,
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
            var upload = await _uploadRepository.GetByIdWithDetailsAsync(uploadId);
            return upload == null ? null : MapToDto(upload);
        }

        public async Task<List<UploadDto>> GetUploadsByCategoryIdAsync(Guid categoryId)
        {
            var uploads = await _uploadRepository.GetByCategoryIdAsync(categoryId);
            return uploads.Select(MapToDto).ToList();
        }

        public async Task<UploadDto> UploadFileAsync(CreateUploadDto createUploadDto, IFormFile file)
        {
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
            var upload = new DataAccessLayer.Models.Upload
            {
                Id = Guid.NewGuid(),
                Name = createUploadDto.Name,
                Img = fileUrl,
                Description = createUploadDto.Description,
                CategoryId = createUploadDto.CategoryId,
                LostLocation = createUploadDto.LostLocation,
                LostDate = createUploadDto.LostDate,
                Content = createUploadDto.Content,
                Status = createUploadDto.Status,
                Staffid = createUploadDto.Staffid,
                Userid = createUploadDto.Userid,
                Type = createUploadDto.Type,
                Note = createUploadDto.Note,
                DateCreate = DateTime.UtcNow,
                DateUpdate = DateTime.UtcNow
            };

            await _uploadRepository.CreateAsync(upload);
            var createdUpload = await _uploadRepository.GetByIdWithDetailsAsync(upload.Id);
            return MapToDto(createdUpload!);
        }

        public async Task<UploadDto?> UpdateUploadAsync(Guid uploadId, UpdateUploadDto updateUploadDto)
        {
            var existingUpload = await _uploadRepository.GetByIdAsync(uploadId);
            if (existingUpload == null)
                return null;

            existingUpload.Name = updateUploadDto.Name;
            existingUpload.Description = updateUploadDto.Description;
            existingUpload.CategoryId = updateUploadDto.CategoryId;
            existingUpload.LostLocation = updateUploadDto.LostLocation;
            existingUpload.LostDate = updateUploadDto.LostDate;
            existingUpload.Content = updateUploadDto.Content;
            existingUpload.Status = updateUploadDto.Status;
            existingUpload.Staffid = updateUploadDto.Staffid;
            existingUpload.Type = updateUploadDto.Type;
            existingUpload.Note = updateUploadDto.Note;
            existingUpload.DateUpdate = DateTime.UtcNow;

            await _uploadRepository.UpdateAsync(existingUpload);
            var updatedUpload = await _uploadRepository.GetByIdWithDetailsAsync(uploadId);
            return MapToDto(updatedUpload!);
        }

        public async Task<bool> DeleteUploadAsync(Guid uploadId)
        {
            var upload = await _uploadRepository.GetByIdAsync(uploadId);
            if (upload == null)
                return false;

            // Delete physical file if exists
            if (!string.IsNullOrEmpty(upload.Img))
            {
                await DeleteFileAsync(upload.Img);
            }

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

        private UploadDto MapToDto(DataAccessLayer.Models.Upload upload)
        {
            return new UploadDto
            {
                Id = upload.Id,
                Name = upload.Name,
                Img = upload.Img,
                Description = upload.Description,
                CategoryId = upload.CategoryId,
                LostLocation = upload.LostLocation,
                LostDate = upload.LostDate,
                Content = upload.Content,
                Status = upload.Status,
                Staffid = upload.Staffid,
                DateCreate = upload.DateCreate,
                Userid = upload.Userid,
                Type = upload.Type,
                Note = upload.Note,
                DateUpdate = upload.DateUpdate,
                CategoryName = upload.Category?.Name,
                UserName = upload.User?.Username,
                StaffName = upload.Staff?.Username
            };
        }
    }
}

