using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.IService
{
    public interface IServiceLocationService
    {
        Task<List<ServiceLocation>> GetAll();
        Task<List <ServiceLocation>> GetAllByCampusId(Guid id);
        Task<ServiceLocation> GetById(Guid id);
        Task<ServiceLocation> Create( ServiceLocationServiceRequest serviceLocation);
        Task<ServiceLocation> Update(Guid id, ServiceLocationServiceRequest serviceLocation);
        Task<bool> Delete(Guid id);

    }
}
