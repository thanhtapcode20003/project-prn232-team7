using BusinessObjectLayer.DTOs.Campus;
using BusinessObjectLayer.IService;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Services
{
    public class CampusService : ICampusService
    {
        private readonly LostAndFoundSystemDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampusService> _logger;
        public CampusService(
            LostAndFoundSystemDbContext context,
            IConfiguration configuration,
            ILogger<CampusService> logger)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public Task<Campus> CreateCampus(CampusRequest campus)
        {
            throw new NotImplementedException();
        }

        public Task<List<Campus>> GetAllCampuses()
        {
            throw new NotImplementedException();
        }

        public Task<Campus> GetCampusById(int campusId)
        {
            throw new NotImplementedException();
        }

        public Task<Campus> UpdateCampus(int campusId, CampusRequest campus)
        {
            throw new NotImplementedException();
        }
    }
}
