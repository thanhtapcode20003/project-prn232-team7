using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.ServiceLocation;
using BusinessObjectLayer.DTOs.ServiceLocationRequest;

namespace BusinessObjectLayer.IService
{
    public interface IServiceLocationService
    {
        Task<List<ServiceLocationResponse>> GetAll();
        Task<List<ServiceLocationResponse>> GetAllByCampusId(Guid id);
        Task<ServiceLocationResponse> GetById(Guid id);
        Task<ServiceLocationResponse> Create(ServiceLocationServiceRequest serviceLocation);
        Task<ServiceLocationResponse> Update(Guid id, ServiceLocationServiceRequest serviceLocation);
        Task<bool> Delete(Guid id);
        Task<PaginationResult<List<ServiceLocationResponse>>> SearchServiceLocationsAsync(ServicelocationFilter filter);

    }
}
