using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.DTOs.ServiceLocation;
using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;

namespace BusinessObjectLayer.Services
{
    public class ServiceLocationService : IServiceLocationService
    {
        private readonly ServiceLocationrepository _serviceLocationrepository;

        public ServiceLocationService()
        {
            _serviceLocationrepository = new ServiceLocationrepository();
        }
        public ServiceLocationService(ServiceLocationrepository serviceLocationrepository)
        {
            _serviceLocationrepository = serviceLocationrepository;
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
                await _serviceLocationrepository.CreateAsync(newServiceLocationService);
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
                var check = await _serviceLocationrepository.GetByIdAsync(id);
                if (check == null)
                {
                    throw new Exception("not found service location with id " + id);
                }
                check.Status = StatusEnum.DELETED.ToString();
                await _serviceLocationrepository.UpdateAsync(check);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(" can not delete this service location" + ex.Message);

            }
        }

        public async Task<List<ServiceLocation>> GetAll()
        {
            try
            {
                return await _serviceLocationrepository.GetAllAsync();
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
                return await _serviceLocationrepository.FindAsync(s => s.CampusId.Equals(id));
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
                return await _serviceLocationrepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<PaginationResult<List<ServiceLocationResponse>>>
 SearchServiceLocationsAsync(ServicelocationFilter filter)
        {
            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 10;

            var (items, totalItems) =
                await _serviceLocationrepository.SearchServiceLocationsAsync(
                    status: StatusEnum.ACTIVE.ToString(),
                    locationName: filter.Name,
                    campusName: filter.CampusName,
                    address: filter.Address,
                    page: filter.Page,
                    pageSize: filter.PageSize
                );

            var itemsDto = items.Select(MapToDTO).ToList();

            return new PaginationResult<List<ServiceLocationResponse>>
            {
                Items = itemsDto,
                TotalItems = totalItems,
                PageSize = filter.PageSize,
                CurrentPage = filter.Page,
                TotalPages = (int)Math.Ceiling(
                    totalItems / (double)filter.PageSize)
            };
        }


        public ServiceLocationResponse MapToDTO(ServiceLocation serviceLocation)
        {
            return new ServiceLocationResponse
            {
                ServiceLocationId = serviceLocation.ServiceLocationId,
                LocationName = serviceLocation.LocationName,
                Campus = new CampusResponse
                {
                    CampusId = serviceLocation.Campus.CampusId,
                    CampusName = serviceLocation.Campus.CampusName,
                    Status = serviceLocation.Campus.Status,
                    Address = serviceLocation.Campus.Address,
                    Description = serviceLocation.Campus.Description,

                },
                Status = serviceLocation.Status
            };
        }

        public async Task<ServiceLocation> Update(Guid id, ServiceLocationServiceRequest serviceLocation)
        {
            try
            {
                var check = await _serviceLocationrepository.GetByIdAsync(id);
                if (check == null)
                {
                    throw new Exception("not found service location");
                }
                check.LocationName = serviceLocation.Name;
                check.CampusId = serviceLocation.CampusId;
                await _serviceLocationrepository.UpdateAsync(check);
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
