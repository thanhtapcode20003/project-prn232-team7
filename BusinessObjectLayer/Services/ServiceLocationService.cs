using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Services
{
    public class ServiceLocationService : IServiceLocationService
    {
        private readonly GenericRepository<ServiceLocation> _genericRepository;

        public ServiceLocationService()
        {
            _genericRepository = new GenericRepository<ServiceLocation>();
        }
        public ServiceLocationService(GenericRepository<ServiceLocation> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<ServiceLocation> Create(ServiceLocationServiceRequest serviceLocation)
        {
            try
            {
                var newServiceLocationService = new ServiceLocation
                {
                    LocationName = serviceLocation.Name,
                    CampusId = serviceLocation.CampusId,
                    Status = StatusEnum.ACTIVE.ToString()
                };
                await _genericRepository.CreateAsync(newServiceLocationService);
                return newServiceLocationService;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the service location.", ex);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var check = await _genericRepository.GetByIdAsync(id);
                if (check == null)
                {
                    throw new Exception("not found service location with id " + id);
                }
                check.Status = StatusEnum.DELETED.ToString();
                await _genericRepository.UpdateAsync(check);
                return true;
            }
            catch (Exception ex) {
                throw new Exception(" can not delete this service location"+ ex.Message);
                    
            }
        }

        public async Task<List<ServiceLocation>> GetAll()
        {
            try
            {
                return await _genericRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("can not get all service location" + ex.Message);
            }
        }

        public async Task<List<ServiceLocation>> GetAllByCampusId(Guid id)
        {
            try
            {
                return await _genericRepository.FindAsync(s => s.CampusId.Equals(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ServiceLocation> GetById(Guid id)
        {
            try
            {
                return await _genericRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<ServiceLocation> Update(Guid id, ServiceLocationServiceRequest serviceLocation)
        {
            try
            {
                var check = await _genericRepository.GetByIdAsync(id);
                if (check == null)
                {
                    throw new Exception("not found service location");
                }
                check.LocationName = serviceLocation.Name;
                check.CampusId = serviceLocation.CampusId;
                await _genericRepository.UpdateAsync(check);
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
