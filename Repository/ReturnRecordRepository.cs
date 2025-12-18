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

        public async Task<List<ReturnRecord>> GetAllWithDetailsAsync()
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<ReturnRecord?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ReturnRecord>> GetByItemIdAsync(Guid itemId)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .Where(r => r.ItemId == itemId)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> GetByStaffIdAsync(Guid staffId)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .Where(r => r.StaffId == staffId)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> GetByStatusAsync(string status)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ReturnRecords.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> ItemExistsAsync(Guid itemId)
        {
            return await _context.Items.AnyAsync(i => i.Id == itemId);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<List<ReturnRecord>> SearchReturnRecordsAsync(
            string? status = null,
            Guid? itemId = null,
            Guid? staffId = null,
            Guid? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.Staff)
                .Include(r => r.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (itemId.HasValue)
                query = query.Where(r => r.ItemId == itemId.Value);

            if (staffId.HasValue)
                query = query.Where(r => r.StaffId == staffId.Value);

            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId.Value);

            if (fromDate.HasValue)
                query = query.Where(r => r.DateCreated >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.DateCreated <= toDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r =>
                    (r.Item != null && r.Item.Name != null && r.Item.Name.Contains(searchTerm)) ||
                    (r.Staff != null && r.Staff.Username != null && r.Staff.Username.Contains(searchTerm)) ||
                    (r.User != null && r.User.Username != null && r.User.Username.Contains(searchTerm)) ||
                    (r.Status != null && r.Status.Contains(searchTerm)));
            }

            // Pagination
            query = query
                .OrderByDescending(r => r.DateCreated)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountReturnRecordsAsync(
            string? status = null,
            Guid? itemId = null,
            Guid? staffId = null,
            Guid? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null)
        {
            var query = _context.ReturnRecords.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (itemId.HasValue)
                query = query.Where(r => r.ItemId == itemId.Value);

            if (staffId.HasValue)
                query = query.Where(r => r.StaffId == staffId.Value);

            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId.Value);

            if (fromDate.HasValue)
                query = query.Where(r => r.DateCreated >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.DateCreated <= toDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r =>
                    (r.Status != null && r.Status.Contains(searchTerm)) ||
                    _context.Items.Any(i => i.Id == r.ItemId && i.Name.Contains(searchTerm)) ||
                    _context.Users.Any(u => u.Id == r.StaffId && u.Username.Contains(searchTerm)) ||
                    _context.Users.Any(u => u.Id == r.UserId && u.Username.Contains(searchTerm)));
            }

            return await query.CountAsync();
        }
    }
}

