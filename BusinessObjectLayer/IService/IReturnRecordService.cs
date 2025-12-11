using BusinessObjectLayer.DTOs.ReturnRecord;

namespace BusinessObjectLayer.IService
{
    public interface IReturnRecordService
    {
        Task<PagedResult<ReturnRecordDto>> SearchReturnRecordsAsync(ReturnRecordFilterDto filter);
        Task<ReturnRecordDto?> GetReturnRecordByIdAsync(Guid id);
        Task<ReturnRecordDto> CreateReturnRecordAsync(CreateReturnRecordDto createReturnRecordDto);
        Task<ReturnRecordDto?> UpdateReturnRecordAsync(Guid id, UpdateReturnRecordDto updateReturnRecordDto);
        Task<bool> DeleteReturnRecordAsync(Guid id);
    }
}

