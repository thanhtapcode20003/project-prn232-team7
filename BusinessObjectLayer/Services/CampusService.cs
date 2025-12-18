using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;

namespace BusinessObjectLayer.Services
{
    public class CampusService : ICampusService
    {
        private readonly CampusRepository _campusRepository;

        public CampusService()
        {
            _campusRepository = new CampusRepository();
        }
        public CampusService(CampusRepository campusRepository)
        {
            _campusRepository = campusRepository;
        }

        public async Task<CampusResponse> CreateCampus(CampusRequest campus)
        {
            var newCampus = new Campus
            {
                Name = campus.Name,
                Address = campus.Address,
                Description = campus.Description,
                Datecreate = DateTime.UtcNow,
                Status = StatusEnum.ACTIVE.ToString()
            };

            await _campusRepository.CreateAsync(newCampus);
            return MapToDTO(newCampus);
        }

        public async Task<bool> DeleteCampus(Guid campusId)
        {
            var campus = await _campusRepository.GetByIdAsync(campusId);

            if (campus == null || !IsActive(campus.Status))
            {
                throw new NotFoundException("Campus", campusId.ToString());
            }

            campus.Status = StatusEnum.INACTIVE.ToString();
            await _campusRepository.UpdateAsync(campus);

            return true;
        }

        public async Task<List<CampusResponse>> GetAllCampuses()
        {
            var listcampus = await _campusRepository.FindAsync(
                c => c.Status == StatusEnum.ACTIVE.ToString()
            );
            return listcampus.Select(MapToDTO).ToList();
        }

        public async Task<CampusResponse> GetCampusById(Guid campusId)
        {
            var campus = await _campusRepository.GetByIdAsync(campusId);

            if (campus == null || !IsActive(campus.Status))
            {
                throw new NotFoundException("Campus", campusId.ToString());
            }

            return MapToDTO(campus);
        }

        public async Task<CampusResponse> UpdateCampus(Guid campusId, CampusRequest campus)
        {
            var campusToUpdate = await _campusRepository.GetByIdAsync(campusId);

            if (campusToUpdate == null || !IsActive(campusToUpdate.Status))
            {
                throw new NotFoundException("Campus", campusId.ToString());
            }

            campusToUpdate.Name = campus.Name;
            campusToUpdate.Address = campus.Address;
            campusToUpdate.Description = campus.Description;
            await _campusRepository.UpdateAsync(campusToUpdate);
            return MapToDTO(campusToUpdate);
        }
        public async Task<PaginationResult<List<CampusResponse>>> SearchCampuses(CampusFilterDto filterDto)
        {
            if (filterDto.Page <= 0) filterDto.Page = 1;
            if (filterDto.PageSize <= 0) filterDto.PageSize = 10;

            var (items, totalItems) =
                await _campusRepository.SearchCampusesAsync(
                    status: StatusEnum.ACTIVE.ToString(),
                    nameContains: filterDto.Name,
                    page: filterDto.Page,
                    pageSize: filterDto.PageSize
                );
            var itemsDto = items.Select(MapToDTO).ToList();
            return new PaginationResult<List<CampusResponse>>
            {
                Items = itemsDto,
                TotalItems = totalItems,
                PageSize = filterDto.PageSize,
                CurrentPage = filterDto.Page,
                TotalPages = (int)Math.Ceiling(
                    totalItems / (double)filterDto.PageSize)
            };
        }

        public CampusResponse MapToDTO(Campus campus)
        {
            return new CampusResponse
            {
                CampusId = campus.Id,
                CampusName = campus.Name,
                Location = campus.Address,
                Description = campus.Description,
                Status = campus.Status
            };
        }
        private static bool IsActive(string? status)
        {
            return string.Equals(
                status,
                StatusEnum.ACTIVE.ToString(),
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}
