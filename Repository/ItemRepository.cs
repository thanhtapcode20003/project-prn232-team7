using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ItemRepository : GenericRepository<Item>
    {
        public ItemRepository() : base()
        {
        }

        public ItemRepository(LostAndFoundSystemDbContext context) : base(context)
        {
        }

        public async Task<List<Item>> GetAllWithDetailsAsync()
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .ToListAsync();
        }

        public async Task<Item?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Include(i => i.Uploads)
                .Include(i => i.ReturnRecords)
                .FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<List<Item>> GetByStatusAsync(string status)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Where(i => i.Status == status)
                .ToListAsync();
        }

        public async Task<List<Item>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Item>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Where(i => i.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Item>> GetByLocationIdAsync(Guid locationId)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Where(i => i.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Items.AnyAsync(i => i.ItemId == id);
        }

        // Search items by name
        public async Task<List<Item>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Where(i => i.ItemName.Contains(searchTerm))
                .ToListAsync();
        }

        // Get items by date range
        public async Task<List<Item>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .Where(i => i.LostDate >= startDate && i.LostDate <= endDate)
                .ToListAsync();
        }
        public async Task<List<Item>> SearchItemsAsync(
            string? status = null,
            Guid? userId = null,
            Guid? categoryId = null,
            Guid? locationId = null,
            string? searchTerm = null,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .Include(i => i.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (userId.HasValue)
                query = query.Where(i => i.UserId == userId.Value);

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryId == categoryId.Value);

            if (locationId.HasValue)
                query = query.Where(i => i.LocationId == locationId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(i =>
                    i.ItemName.Contains(searchTerm) ||
                    (i.Description != null && i.Description.Contains(searchTerm)));

            if (fromDate.HasValue)
                query = query.Where(i => i.LostDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.LostDate <= toDate.Value);

            // Pagination
            query = query
                .OrderByDescending(i => i.LostDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        // ✅ Count total cho pagination
        public async Task<int> CountItemsAsync(
            string? status = null,
            Guid? userId = null,
            Guid? categoryId = null,
            Guid? locationId = null,
            string? searchTerm = null,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (userId.HasValue)
                query = query.Where(i => i.UserId == userId.Value);

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryId == categoryId.Value);

            if (locationId.HasValue)
                query = query.Where(i => i.LocationId == locationId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(i =>
                    i.ItemName.Contains(searchTerm) ||
                    (i.Description != null && i.Description.Contains(searchTerm)));

            if (fromDate.HasValue)
                query = query.Where(i => i.LostDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.LostDate <= toDate.Value);

            return await query.CountAsync();
        }
    }
}