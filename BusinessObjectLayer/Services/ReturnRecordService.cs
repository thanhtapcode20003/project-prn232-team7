using BusinessObjectLayer.DTOs.ReturnRecord;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace BusinessObjectLayer.Services
{
    public class ReturnRecordService : IReturnRecordService
    {
        private readonly ReturnRecordRepository _returnRecordRepository;
        private readonly ItemRepository _itemRepository;
        private readonly IAuthService _authService;
        private readonly LostAndFoundDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const string UploadFolder = "uploads";
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };

        public ReturnRecordService(
            ReturnRecordRepository returnRecordRepository,
            ItemRepository itemRepository,
            IAuthService authService,
            IWebHostEnvironment environment,
            LostAndFoundDbContext lostAndFoundDbContext
            )
        {
            _returnRecordRepository = returnRecordRepository;
            _itemRepository = itemRepository;
            _authService = authService;
            _environment = environment;
            _context = lostAndFoundDbContext;
        }

        public async Task<List<ReturnRecordDto>> GetAllAsync()
        {
            var records = await _returnRecordRepository.GetAllWithDetailsAsync();
            return records.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<ReturnRecordDto>> SearchAsync(ReturnRecordFilterDto filter)
        {
            var records = await _returnRecordRepository.SearchAsync(
                status: StatusEnum.ACTIVE.ToString(),

                itemName: filter.NameItem,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _returnRecordRepository.CountAsync(
                itemName: filter.NameItem,
                fromDate: filter.FromDate,
                toDate: filter.ToDate
            );

            return new PagedResult<ReturnRecordDto>
            {
                Items = records.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ReturnRecordDto?> GetByIdAsync(Guid id)
        {
            var record = await _returnRecordRepository.GetByIdWithDetailsAsync(id);
            return record == null ? null : MapToDto(record);
        }

        public async Task<ReturnRecordDto> CreateAsync(CreateReturnRecordDto dto)
        {
            var user = _authService.GetCurrentUser();

            var item = await _itemRepository.GetByIdAsync(dto.ItemId);
            if (item == null)
            {
                throw new NotFoundException("Item not found with id", dto.ItemId.ToString());
            }
            if (StatusEnum.ACTIVE.ToString() != item.Status)
            {
                throw new BadHttpRequestException("item has reurned or delete");
            }

            // Lưu file ảnh nếu có
            var imgFontPath = dto.ImgCccdFont != null ? await SaveFileAsync(dto.ImgCccdFont) : null;
            var imgBackPath = dto.ImgCccdBack != null ? await SaveFileAsync(dto.ImgCccdBack) : null;
            var evidencePath = dto.EvidenceImg != null ? await SaveFileAsync(dto.EvidenceImg) : null;
            var confirmPath = dto.ConfirmImg != null ? await SaveFileAsync(dto.ConfirmImg) : null;

            var record = new ReturnRecord
            {
                Id = Guid.NewGuid(),
                ItemId = dto.ItemId,
                StaffId = user.Id,

                ImgCccdFont = imgFontPath,
                ImgCccdBack = imgBackPath,
                EvidenceImg = evidencePath,
                ConfirmImg = confirmPath,
                VerifyNotes = dto.VerifyNotes,
                Status = StatusEnum.ACTIVE.ToString(),
                DateCreated = DateTime.UtcNow,
                DateUpdate = DateTime.UtcNow
            };

            await CreateReturnRecordWithTransactionAsync(record, item);
            var created = await _returnRecordRepository.GetByIdWithDetailsAsync(record.Id);
            return MapToDto(created!);
        }
        private async Task CreateReturnRecordWithTransactionAsync(ReturnRecord record, Item item)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _returnRecordRepository.CreateAsync(record);

                    item.Status = StatusEnum.RETURNED.ToString();
                    await _itemRepository.UpdateAsync(item);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }



        public async Task<ReturnRecordDto?> UpdateAsync(Guid id, UpdateReturnRecordDto dto)
        {
            var record = await _returnRecordRepository.GetByIdWithDetailsAsync(id);
            if (record == null)
            {
                return null;
            }

            // Nếu gửi file mới thì lưu và cập nhật path, nếu không thì giữ nguyên
            if (dto.ImgCccdFont != null)
            {
                record.ImgCccdFont = await SaveFileAsync(dto.ImgCccdFont);
            }

            if (dto.ImgCccdBack != null)
            {
                record.ImgCccdBack = await SaveFileAsync(dto.ImgCccdBack);
            }

            if (dto.EvidenceImg != null)
            {
                record.EvidenceImg = await SaveFileAsync(dto.EvidenceImg);
            }

            if (dto.ConfirmImg != null)
            {
                record.ConfirmImg = await SaveFileAsync(dto.ConfirmImg);
            }

            record.VerifyNotes = dto.VerifyNotes ?? record.VerifyNotes;

            record.DateUpdate = DateTime.UtcNow;

            await _returnRecordRepository.UpdateAsync(record);
            var updated = await _returnRecordRepository.GetByIdWithDetailsAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var record = await _returnRecordRepository.GetByIdAsync(id);
            if (record == null)
            {
                return false;
            }
            record.Status = StatusEnum.DELETED.ToString();
            await _returnRecordRepository.UpdateAsync(record);
            return true;
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            ValidateFile(file);

            var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về path để lưu trong DB
            return $"/{UploadFolder}/{fileName}";
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

        private ReturnRecordDto MapToDto(ReturnRecord record)
        {
            return new ReturnRecordDto
            {
                Id = record.Id,
                ItemId = record.ItemId,
                StaffId = record.StaffId,

                ImgCccdFont = record.ImgCccdFont,
                ImgCccdBack = record.ImgCccdBack,
                EvidenceImg = record.EvidenceImg,
                ConfirmImg = record.ConfirmImg,
                VerifyNotes = record.VerifyNotes,
                Status = record.Status,
                DateCreated = record.DateCreated,
                DateUpdate = record.DateUpdate,
                ItemName = record.Item?.Name,
                StaffName = record.Staff?.Username,

            };
        }
    }
}
