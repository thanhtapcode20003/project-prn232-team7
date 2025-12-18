using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.DTOs.ServiceLocation;
using BusinessObjectLayer.DTOs.ServiceLocationRequest;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;

namespace BusinessObjectLayer.Services
{
    public class ServiceLocationService : IServiceLocationService
    {
        private readonly ServiceLocationrepository _serviceLocationrepository;

        public ServiceLocationService(ServiceLocationrepository serviceLocationrepository)
        {
            _serviceLocationrepository = serviceLocationrepository;
        }

        /* ===========================
           CREATE
        ============================ */
        public async Task<ServiceLocationResponse> Create(ServiceLocationServiceRequest request)
        {
            try
            {
                var entity = new ServiceLocation
                {
                    Name = request.Name,
                    Address = request.Address,
                    Description = request.Description,
                    CampusId = request.CampusId,
                    Status = StatusEnum.ACTIVE.ToString(),
                    Datecreate = DateTime.UtcNow
                };

                await _serviceLocationrepository.CreateAsync(entity);
                return MapToDTO(entity);
            }
            catch (Exception ex)
            {
                throw new ApiException(
                    ApiError.InternalServerError(
                        "Failed to create service location",
                        ex.Message
                    )
                );
            }
        }

        /* ===========================
           DELETE (Soft Delete)
        ============================ */
        public async Task<bool> Delete(Guid id)
        {
            var serviceLocation = await _serviceLocationrepository.GetByIdAsync(id)
                ?? throw new NotFoundException("ServiceLocation", id.ToString());

            serviceLocation.Status = StatusEnum.DELETED.ToString();

            await _serviceLocationrepository.UpdateAsync(serviceLocation);
            return true;
        }

        /* ===========================
           GET ALL
        ============================ */
        public async Task<List<ServiceLocationResponse>> GetAll()
        {
            try
            {
                var list = await _serviceLocationrepository.GetAllByCampusIdAsync();
                return list.Select(MapToDTO).ToList();
            }
            catch (Exception ex)
            {
                throw new ApiException(
                    ApiError.InternalServerError(
                        "Failed to retrieve service locations",
                        ex.Message
                    )
                );
            }
        }

        /* ===========================
           GET BY CAMPUS ID
        ============================ */
        public async Task<List<ServiceLocationResponse>> GetAllByCampusId(Guid campusId)
        {
            var list = await _serviceLocationrepository.GetAllByCampusIdAsync();
            return list.Select(MapToDTO).ToList();
        }

        /* ===========================
           GET BY ID
        ============================ */
        public async Task<ServiceLocationResponse> GetById(Guid id)
        {
            var serviceLocation = await _serviceLocationrepository.GetByIdWithCampusAsync(id)
                ?? throw new NotFoundException("ServiceLocation", id.ToString());

            return MapToDTO(serviceLocation);
        }

        /* ===========================
           SEARCH + PAGINATION
        ============================ */
        public async Task<PaginationResult<List<ServiceLocationResponse>>>
            SearchServiceLocationsAsync(ServicelocationFilter filter)
        {
            if (filter.Page <= 0 || filter.PageSize <= 0)
            {
                throw new ApiException(
                    ApiError.ValidationError("Page and PageSize must be greater than zero")
                );
            }

            var (items, totalItems) =
                await _serviceLocationrepository.SearchServiceLocationsAsync(
                    status: StatusEnum.ACTIVE.ToString(),
                    locationName: filter.Name,
                    campusName: filter.CampusName,
                    address: filter.Address,
                    page: filter.Page,
                    pageSize: filter.PageSize
                );

            return new PaginationResult<List<ServiceLocationResponse>>
            {
                Items = items.Select(MapToDTO).ToList(),
                TotalItems = totalItems,
                PageSize = filter.PageSize,
                CurrentPage = filter.Page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize)
            };
        }

        /* ===========================
           UPDATE
        ============================ */
        public async Task<ServiceLocationResponse> Update(Guid id, ServiceLocationServiceRequest request)
        {
            var serviceLocation = await _serviceLocationrepository.GetByIdAsync(id)
                ?? throw new NotFoundException("ServiceLocation", id.ToString());

            serviceLocation.Name = request.Name;
            serviceLocation.Address = request.Address;
            serviceLocation.Description = request.Description;
            serviceLocation.CampusId = request.CampusId;
            serviceLocation.Dateupdate = DateTime.UtcNow;

            await _serviceLocationrepository.UpdateAsync(serviceLocation);
            return MapToDTO(serviceLocation);
        }

        /* ===========================
           MAPPER
        ============================ */
        private static ServiceLocationResponse MapToDTO(ServiceLocation serviceLocation)
        {
            return new ServiceLocationResponse
            {
                ServiceLocationId = serviceLocation.Id,
                LocationName = serviceLocation.Name,
                Address = serviceLocation.Address,
                Description = serviceLocation.Description,
                Status = serviceLocation.Status,

                Campus = new CampusResponse
                {
                    CampusId = serviceLocation.Campus.Id,
                    CampusName = serviceLocation.Campus.Name,
                    Status = serviceLocation.Campus.Status,
                    Address = serviceLocation.Campus.Address,
                    Description = serviceLocation.Campus.Description,
                },
            };
        }
    }
}
