using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;

namespace BusinessObjectLayer.IService
{
    public interface ICampusService
    {
        Task<CampusResponse> GetCampusById(Guid campusId);
        Task<List<CampusResponse>> GetAllCampuses();
        Task<CampusResponse> CreateCampus(CampusRequest campus);
        Task<CampusResponse> UpdateCampus(Guid campusId, CampusRequest campus);
        Task<bool> DeleteCampus(Guid campusId);
        Task<PaginationResult<List<CampusResponse>>> SearchCampuses(CampusFilterDto filterDto);
    }
}
