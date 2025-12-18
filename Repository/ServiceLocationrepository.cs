using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public class ServiceLocationrepository : GenericRepository<ServiceLocation>
    {
        public ServiceLocationrepository() : base()
        {
        }
        public ServiceLocationrepository(LostAndFoundDbContext context) : base(context)
        {
        }
        public async Task<List<ServiceLocation>> GetAllByCampusIdAsync(Guid campusId)
        {
            return await _context.ServiceLocations
                .Where(sl => sl.CampusId == campusId)
                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(Guid serviceLocationId)
        {
            return await _context.ServiceLocations.AnyAsync(sl => sl.Id == serviceLocationId);
        }
        public async Task<List<ServiceLocation>> GetAllWithCampusAsync()
        {
            return await _context.ServiceLocations
                .Include(sl => sl.Campus)
                .ToListAsync();
        }
        public async Task<ServiceLocation?> GetByIdWithCampusAsync(Guid serviceLocationId)
        {
            return await _context.ServiceLocations
                .Include(sl => sl.Campus)
                .FirstOrDefaultAsync(sl => sl.Id == serviceLocationId);
        }
        public async Task<(List<ServiceLocation>, int)> SearchServiceLocationsAsync(
      string? status,
      string? locationName,
      string? campusName,
      string? address,
      int page,
      int pageSize)
        {
            var query = _context.ServiceLocations
                .Include(sl => sl.Campus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(sl => sl.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(locationName))
            {
                query = query.Where(sl =>
                    sl.Name.Contains(locationName));
            }

            if (!string.IsNullOrWhiteSpace(campusName))
            {
                query = query.Where(sl =>
                    sl.Campus.Name.Contains(campusName));
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                query = query.Where(sl =>
                    sl.Address != null && sl.Address.Contains(address));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }



    }
}
