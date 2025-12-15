using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.ServiceLocation;
using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using DataAccessLayer.Models;

namespace BusinessObjectLayer.IService
{
    public interface IServiceLocationService
    {
        Task<List<ServiceLocation>> GetAll();
        Task<List<ServiceLocation>> GetAllByCampusId(Guid id);
        Task<ServiceLocation> GetById(Guid id);
        Task<ServiceLocation> Create(ServiceLocationServiceRequest serviceLocation);
        Task<ServiceLocation> Update(Guid id, ServiceLocationServiceRequest serviceLocation);
        Task<bool> Delete(Guid id);
        Task<PaginationResult<List<ServiceLocationResponse>>> SearchServiceLocationsAsync(ServicelocationFilter filter);

    }
}
