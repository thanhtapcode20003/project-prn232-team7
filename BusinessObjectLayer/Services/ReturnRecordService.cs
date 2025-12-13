using BusinessObjectLayer.DTOs.ReturnRecord;
using BusinessObjectLayer.IService;
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
                foundUserId: filter.FoundUserId,
                receiverUserId: filter.ReceiverUserId,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                searchTerm: filter.SearchTerm,
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize
            );

            var totalCount = await _returnRecordRepository.CountReturnRecordsAsync(
                status: filter.Status,
                itemId: filter.ItemId,
                foundUserId: filter.FoundUserId,
                receiverUserId: filter.ReceiverUserId,
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
            var returnRecord = new ReturnRecord
            {
                ReturnId = Guid.NewGuid(),
                ItemId = createReturnRecordDto.ItemId,
                FoundUserId = createReturnRecordDto.FoundUserId,
                ReceiverUserId = createReturnRecordDto.ReceiverUserId,
                ReturnDate = createReturnRecordDto.ReturnDate ?? DateTime.Now,
                Status = createReturnRecordDto.Status
            };

            await _returnRecordRepository.CreateAsync(returnRecord);
            var createdReturnRecord = await _returnRecordRepository.GetByIdWithDetailsAsync(returnRecord.ReturnId);
            return MapToDto(createdReturnRecord!);
        }

        public async Task<ReturnRecordDto?> UpdateReturnRecordAsync(Guid id, UpdateReturnRecordDto updateReturnRecordDto)
        {
            var existingReturnRecord = await _returnRecordRepository.GetByIdAsync(id);
            if (existingReturnRecord == null)
                return null;

            existingReturnRecord.ItemId = updateReturnRecordDto.ItemId;
            existingReturnRecord.FoundUserId = updateReturnRecordDto.FoundUserId;
            existingReturnRecord.ReceiverUserId = updateReturnRecordDto.ReceiverUserId;
            if (updateReturnRecordDto.ReturnDate.HasValue)
                existingReturnRecord.ReturnDate = updateReturnRecordDto.ReturnDate.Value;
            existingReturnRecord.Status = updateReturnRecordDto.Status;

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
                ReturnId = returnRecord.ReturnId,
                ItemId = returnRecord.ItemId,
                FoundUserId = returnRecord.FoundUserId,
                ReceiverUserId = returnRecord.ReceiverUserId,
                ReturnDate = returnRecord.ReturnDate,
                Status = returnRecord.Status,
                ItemName = returnRecord.Item?.ItemName,
                FoundUserName = returnRecord.FoundUser?.Username,
                ReceiverUserName = returnRecord.ReceiverUser?.Username
            };
        }
    }
}

