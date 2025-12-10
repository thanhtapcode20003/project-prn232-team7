using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.IService;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Services
{
    public class CampusService : ICampusService
    {

        private readonly GenericRepository<Campus> _campusRepository;
        public CampusService()
        {
            _campusRepository = new GenericRepository<Campus>();
        }

        public CampusService(GenericRepository<Campus> campusRepository)
        {
            _campusRepository = campusRepository;

        }

        public async Task<Campus> CreateCampus(CampusRequest campus)
        {
            try
            {
                var newCampus = new Campus
                {
                    CampusName = campus.Name,
                    Status = StatusEnum.ACTIVE.ToString(),
                };
                _campusRepository.Create(newCampus);
                return newCampus;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the campus.", ex);
            }
        }

        public async Task<bool> DeleteCampus(Guid campusId)
        {
            try
            {
                var campusToDelete = await _campusRepository.GetByIdAsync(campusId);
                if(campusToDelete == null || !campusToDelete.Status.Equals(StatusEnum.ACTIVE.ToString()))
                {
                    throw new Exception("Campus not found.");
                }
                campusToDelete.Status = StatusEnum.INACTIVE.ToString();
                _campusRepository.Update(campusToDelete);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the campus.", ex);
            }
        }

        public async Task<List<Campus>> GetAllCampuses()
        {
            try
            {
                var campuses =await _campusRepository.FindAsync(c => c.Status.Equals(StatusEnum.ACTIVE.ToString()));
                return campuses;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving campuses.", ex);
            }
        }

        public async Task<Campus> GetCampusById(Guid campusId)
        {
            try
            {
                var campus = await _campusRepository.GetByIdAsync(campusId);
                if(campus == null || !campus.Status.Equals(StatusEnum.ACTIVE.ToString()))
                {
                    throw new Exception("Campus not found.");
                }
                return campus;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the campus.", ex);
            }

        }
        

        public async Task<Campus> UpdateCampus(Guid campusId, CampusRequest campus)
        {
            try
            {
                var campusToUpdate = await _campusRepository.GetByIdAsync(campusId);
                if(campusToUpdate == null || !campusToUpdate.Status.Equals(StatusEnum.ACTIVE.ToString()))
                {
                    throw new Exception("Campus not found.");
                }
                campusToUpdate.CampusName = campus.Name;
                
                _campusRepository.Update(campusToUpdate);

                return campusToUpdate;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the campus.", ex);
            }
        }
    }
}
