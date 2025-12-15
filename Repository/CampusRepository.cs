using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CampusRepository : GenericRepository<Campus>
    {
        public CampusRepository() : base()
        {
        }
        public CampusRepository(LostAndFoundSystemDbContext context) : base(context)
        {
        }
        public async Task<List<Campus>> GetByStatusAsync(string status)
        {
            return await _context.Campuses
                .Where(c => c.Status == status)
                .ToListAsync();
        }
        public async Task<Campus?> GetByNameAsync(string campusName)
        {
            return await _context.Campuses
                .FirstOrDefaultAsync(c => c.CampusName == campusName);
        }
        public async Task<bool> ExistsAsync(Guid campusId)
        {
            return await _context.Campuses.AnyAsync(c => c.CampusId == campusId);
        }
        public async Task<bool> NameExistsAsync(string campusName)
        {
            return await _context.Campuses.AnyAsync(c => c.CampusName == campusName);
        }
        public async Task<List<Campus>> GetAllWithLocationsAsync()
        {
            return await _context.Campuses
                .Include(c => c.ServiceLocations)
                .ToListAsync();
        }
        public async Task<Campus?> GetByIdWithLocationsAsync(Guid campusId)
        {
            return await _context.Campuses
                .Include(c => c.ServiceLocations)
                .FirstOrDefaultAsync(c => c.CampusId == campusId);
        }
        public async Task<(List<Campus> Items, int TotalItems)> SearchCampusesAsync(
         string? status,
         string? nameContains,
         int page,
         int pageSize)
        {
            var query = _context.Campuses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(nameContains))
            {
                query = query.Where(c =>
                    c.CampusName.Contains(nameContains));
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
