using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReturnRecordRepository : GenericRepository<ReturnRecord>
    {
        public ReturnRecordRepository() : base()
        {
        }

        public ReturnRecordRepository(LostAndFoundDbContext context) : base(context)
        {
        }

        public async Task<ReturnRecord?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                    .ThenInclude(i => i.User)
                .Include(r => r.Staff)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ReturnRecord>> GetAllWithDetailsAsync()
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                    .ThenInclude(i => i.User)
                .Include(r => r.Staff)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> SearchAsync(
            string? status = null,
            Guid? userId = null,
            Guid? staffId = null,
            Guid? itemId = null,
            string? itemName = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.ReturnRecords
                .Include(r => r.Item)
                    .ThenInclude(i => i.User)
                .Include(r => r.Staff)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (userId.HasValue)
                query = query.Where(r => r.Item.UserId == userId.Value);

            if (staffId.HasValue)
                query = query.Where(r => r.StaffId == staffId.Value);

            if (itemId.HasValue)
                query = query.Where(r => r.ItemId == itemId.Value);

            if (!string.IsNullOrWhiteSpace(itemName))
            {
                query = query.Where(r =>
                    r.Item != null &&
                    r.Item.Name.Contains(itemName));
            }

            if (fromDate.HasValue)
                query = query.Where(r => r.DateCreated >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.DateCreated <= toDate.Value);

            query = query
                .OrderByDescending(r => r.DateCreated)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(
            string? status = null,
            Guid? userId = null,
            Guid? staffId = null,
            Guid? itemId = null,
            string? itemName = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.ReturnRecords
                .Include(r => r.Item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (userId.HasValue)
                query = query.Where(r => r.Item.UserId == userId.Value);

            if (staffId.HasValue)
                query = query.Where(r => r.StaffId == staffId.Value);

            if (itemId.HasValue)
                query = query.Where(r => r.ItemId == itemId.Value);

            if (!string.IsNullOrWhiteSpace(itemName))
            {
                query = query.Where(r =>
                    r.Item != null &&
                    r.Item.Name.Contains(itemName));
            }

            if (fromDate.HasValue)
                query = query.Where(r => r.DateCreated >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.DateCreated <= toDate.Value);

            return await query.CountAsync();
        }
    }
}
