using BusinessObjectLayer.DTOs.ReturnRecord;

namespace BusinessObjectLayer.IService
{
    public interface IReturnRecordService
    {
        Task<List<ReturnRecordDto>> GetAllAsync();
        Task<PagedResult<ReturnRecordDto>> SearchAsync(ReturnRecordFilterDto filter);
        Task<ReturnRecordDto?> GetByIdAsync(Guid id);
        Task<ReturnRecordDto> CreateAsync(CreateReturnRecordDto dto);
        Task<ReturnRecordDto?> UpdateAsync(Guid id, UpdateReturnRecordDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
