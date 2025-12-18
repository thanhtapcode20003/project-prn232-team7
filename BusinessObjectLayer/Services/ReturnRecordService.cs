using BusinessObjectLayer.DTOs.ReturnRecord;
using BusinessObjectLayer.IService;
using BusinessObjectLayer.Exceptions;
using DataAccessLayer.Models;
using Repository;
using System.Linq;

namespace BusinessObjectLayer.Services
{
    public class ReturnRecordService : IReturnRecordService
    {
        private readonly ReturnRecordRepository _returnRecordRepository;

        public ReturnRecordService()
        {
            _returnRecordRepository = new ReturnRecordRepository();
        }

        public ReturnRecordService(ReturnRecordRepository returnRecordRepository)
        {
            _returnRecordRepository = returnRecordRepository;
        }

        public async Task<PagedResult<ReturnRecordDto>> SearchReturnRecordsAsync(ReturnRecordFilterDto filter)
        {
            var returnRecords = await _returnRecordRepository.SearchReturnRecordsAsync(
                status: filter.Status,
                itemId: filter.ItemId,
                staffId: filter.StaffId,
                userId: filter.UserId,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                searchTerm: filter.SearchTerm,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _returnRecordRepository.CountReturnRecordsAsync(
                status: filter.Status,
                itemId: filter.ItemId,
                staffId: filter.StaffId,
                userId: filter.UserId,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                searchTerm: filter.SearchTerm
            );

            return new PagedResult<ReturnRecordDto>
            {
                Items = returnRecords.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ReturnRecordDto?> GetReturnRecordByIdAsync(Guid id)
        {
            var returnRecord = await _returnRecordRepository.GetByIdWithDetailsAsync(id);
            return returnRecord == null ? null : MapToDto(returnRecord);
        }

        public async Task<ReturnRecordDto> CreateReturnRecordAsync(CreateReturnRecordDto createReturnRecordDto)
        {
            // Validate Item exists
            var itemExists = await _returnRecordRepository.ItemExistsAsync(createReturnRecordDto.ItemId);
            if (!itemExists)
                throw new NotFoundException("Item", createReturnRecordDto.ItemId.ToString());

            // Validate Staff exists
            var staffExists = await _returnRecordRepository.UserExistsAsync(createReturnRecordDto.StaffId);
            if (!staffExists)
                throw new NotFoundException("Staff User", createReturnRecordDto.StaffId.ToString());

            // Validate User exists
            var userExists = await _returnRecordRepository.UserExistsAsync(createReturnRecordDto.UserId);
            if (!userExists)
                throw new NotFoundException("User", createReturnRecordDto.UserId.ToString());

            var returnRecord = new ReturnRecord
            {
                Id = Guid.NewGuid(),
                ItemId = createReturnRecordDto.ItemId,
                StaffId = createReturnRecordDto.StaffId,
                UserId = createReturnRecordDto.UserId,
                ImgCccdFont = createReturnRecordDto.ImgCccdFont,
                ImgCccdBack = createReturnRecordDto.ImgCccdBack,
                EvidenceImg = createReturnRecordDto.EvidenceImg,
                ConfirmImg = createReturnRecordDto.ConfirmImg,
                VerifyNotes = createReturnRecordDto.VerifyNotes,
                Status = createReturnRecordDto.Status,
                DateCreated = DateTime.Now,
                DateUpdate = DateTime.Now
            };

            await _returnRecordRepository.CreateAsync(returnRecord);
            var createdReturnRecord = await _returnRecordRepository.GetByIdWithDetailsAsync(returnRecord.Id);
            return MapToDto(createdReturnRecord!);
        }

        public async Task<ReturnRecordDto?> UpdateReturnRecordAsync(Guid id, UpdateReturnRecordDto updateReturnRecordDto)
        {
            var existingReturnRecord = await _returnRecordRepository.GetByIdAsync(id);
            if (existingReturnRecord == null)
                return null;

            // Validate Item exists
            var itemExists = await _returnRecordRepository.ItemExistsAsync(updateReturnRecordDto.ItemId);
            if (!itemExists)
                throw new NotFoundException("Item", updateReturnRecordDto.ItemId.ToString());

            // Validate Staff exists
            var staffExists = await _returnRecordRepository.UserExistsAsync(updateReturnRecordDto.StaffId);
            if (!staffExists)
                throw new NotFoundException("Staff User", updateReturnRecordDto.StaffId.ToString());

            // Validate User exists
            var userExists = await _returnRecordRepository.UserExistsAsync(updateReturnRecordDto.UserId);
            if (!userExists)
                throw new NotFoundException("User", updateReturnRecordDto.UserId.ToString());

            existingReturnRecord.ItemId = updateReturnRecordDto.ItemId;
            existingReturnRecord.StaffId = updateReturnRecordDto.StaffId;
            existingReturnRecord.UserId = updateReturnRecordDto.UserId;
            existingReturnRecord.ImgCccdFont = updateReturnRecordDto.ImgCccdFont;
            existingReturnRecord.ImgCccdBack = updateReturnRecordDto.ImgCccdBack;
            existingReturnRecord.EvidenceImg = updateReturnRecordDto.EvidenceImg;
            existingReturnRecord.ConfirmImg = updateReturnRecordDto.ConfirmImg;
            existingReturnRecord.VerifyNotes = updateReturnRecordDto.VerifyNotes;
            existingReturnRecord.Status = updateReturnRecordDto.Status;
            existingReturnRecord.DateUpdate = DateTime.Now;

            await _returnRecordRepository.UpdateAsync(existingReturnRecord);
            var updatedReturnRecord = await _returnRecordRepository.GetByIdWithDetailsAsync(id);
            return MapToDto(updatedReturnRecord!);
        }

        public async Task<bool> DeleteReturnRecordAsync(Guid id)
        {
            var returnRecord = await _returnRecordRepository.GetByIdAsync(id);
            if (returnRecord == null)
                return false;

            await _returnRecordRepository.RemoveAsync(returnRecord);
            return true;
        }

        private ReturnRecordDto MapToDto(ReturnRecord returnRecord)
        {
            return new ReturnRecordDto
            {
                Id = returnRecord.Id,
                ItemId = returnRecord.ItemId,
                StaffId = returnRecord.StaffId,
                UserId = returnRecord.UserId,
                ImgCccdFont = returnRecord.ImgCccdFont,
                ImgCccdBack = returnRecord.ImgCccdBack,
                EvidenceImg = returnRecord.EvidenceImg,
                ConfirmImg = returnRecord.ConfirmImg,
                VerifyNotes = returnRecord.VerifyNotes,
                Status = returnRecord.Status,
                DateCreated = returnRecord.DateCreated,
                DateUpdate = returnRecord.DateUpdate,
                ItemName = returnRecord.Item?.Name,
                StaffName = returnRecord.Staff?.Username,
                UserName = returnRecord.User?.Username
            };
        }
    }
}

