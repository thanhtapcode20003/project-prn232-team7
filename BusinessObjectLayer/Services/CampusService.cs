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

        public async Task<Campus> CreateCampus(CampusRequest campus)
        {
            // (Optional) check trùng tên
            if (await _campusRepository.NameExistsAsync(campus.Name))
            {
                throw new Exception("Campus name already exists");
            }

            var newCampus = new Campus
            {
                CampusName = campus.Name,
                Status = StatusEnum.ACTIVE.ToString()
            };

            await _campusRepository.CreateAsync(newCampus);
            return newCampus;
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

        public async Task<List<Campus>> GetAllCampuses()
        {
            return await _campusRepository.FindAsync(
                c => c.Status == StatusEnum.ACTIVE.ToString()
            );
        }

        public async Task<Campus> GetCampusById(Guid campusId)
        {
            var campus = await _campusRepository.GetByIdAsync(campusId);

            if (campus == null || !IsActive(campus.Status))
            {
                throw new NotFoundException("Campus", campusId.ToString());
            }

            return campus;
        }

        public async Task<Campus> UpdateCampus(Guid campusId, CampusRequest campus)
        {
            var campusToUpdate = await _campusRepository.GetByIdAsync(campusId);

            if (campusToUpdate == null || !IsActive(campusToUpdate.Status))
            {
                throw new NotFoundException("Campus", campusId.ToString());
            }

            campusToUpdate.CampusName = campus.Name;
            await _campusRepository.UpdateAsync(campusToUpdate);

            return campusToUpdate;
        }
        public async Task<PaginationResult<List<Campus>>> SearchCampuses(CampusFilterDto filterDto)
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

            return new PaginationResult<List<Campus>>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = filterDto.PageSize,
                CurrentPage = filterDto.Page,
                TotalPages = (int)Math.Ceiling(
                    totalItems / (double)filterDto.PageSize)
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
