using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Campus;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.IService
{
    public interface ICampusService
    {
        Task<Campus> GetCampusById(Guid campusId);
        Task<List<Campus>> GetAllCampuses();
        Task<Campus> CreateCampus(CampusRequest campus);
        Task<Campus> UpdateCampus(Guid campusId, CampusRequest campus);
        Task<bool> DeleteCampus(Guid campusId);
        Task<PaginationResult<List<Campus>>> SearchCampuses(CampusFilterDto filterDto);
    }
}
