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

        public ReturnRecordRepository(LostAndFoundSystemDbContext context) : base(context)
        {
        }

        public async Task<List<ReturnRecord>> GetAllWithDetailsAsync()
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .ToListAsync();
        }

        public async Task<ReturnRecord?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .FirstOrDefaultAsync(r => r.ReturnId == id);
        }

        public async Task<List<ReturnRecord>> GetByItemIdAsync(Guid itemId)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .Where(r => r.ItemId == itemId)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> GetByFoundUserIdAsync(Guid foundUserId)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .Where(r => r.FoundUserId == foundUserId)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> GetByReceiverUserIdAsync(Guid receiverUserId)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .Where(r => r.ReceiverUserId == receiverUserId)
                .ToListAsync();
        }

        public async Task<List<ReturnRecord>> GetByStatusAsync(string status)
        {
            return await _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ReturnRecords.AnyAsync(r => r.ReturnId == id);
        }

        public async Task<bool> ItemExistsAsync(Guid itemId)
        {
            return await _context.Items.AnyAsync(i => i.ItemId == itemId);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task<List<ReturnRecord>> SearchReturnRecordsAsync(
            string? status = null,
            Guid? itemId = null,
            Guid? foundUserId = null,
            Guid? receiverUserId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.ReturnRecords
                .Include(r => r.Item)
                .Include(r => r.FoundUser)
                .Include(r => r.ReceiverUser)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (itemId.HasValue)
                query = query.Where(r => r.ItemId == itemId.Value);

            if (foundUserId.HasValue)
                query = query.Where(r => r.FoundUserId == foundUserId.Value);

            if (receiverUserId.HasValue)
                query = query.Where(r => r.ReceiverUserId == receiverUserId.Value);

            if (fromDate.HasValue)
                query = query.Where(r => r.ReturnDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.ReturnDate <= toDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r =>
                    (r.Item != null && r.Item.ItemName != null && r.Item.ItemName.Contains(searchTerm)) ||
                    (r.FoundUser != null && r.FoundUser.Username != null && r.FoundUser.Username.Contains(searchTerm)) ||
                    (r.ReceiverUser != null && r.ReceiverUser.Username != null && r.ReceiverUser.Username.Contains(searchTerm)) ||
                    r.Status.Contains(searchTerm));
            }

            // Pagination
            query = query
                .OrderByDescending(r => r.ReturnDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountReturnRecordsAsync(
            string? status = null,
            Guid? itemId = null,
            Guid? foundUserId = null,
            Guid? receiverUserId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null)
        {
            var query = _context.ReturnRecords.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (itemId.HasValue)
                query = query.Where(r => r.ItemId == itemId.Value);

            if (foundUserId.HasValue)
                query = query.Where(r => r.FoundUserId == foundUserId.Value);

            if (receiverUserId.HasValue)
                query = query.Where(r => r.ReceiverUserId == receiverUserId.Value);

            if (fromDate.HasValue)
                query = query.Where(r => r.ReturnDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.ReturnDate <= toDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r =>
                    r.Status.Contains(searchTerm) ||
                    _context.Items.Any(i => i.ItemId == r.ItemId && i.ItemName.Contains(searchTerm)) ||
                    _context.Users.Any(u => u.UserId == r.FoundUserId && u.Username.Contains(searchTerm)) ||
                    (r.ReceiverUserId != null && _context.Users.Any(u => u.UserId == r.ReceiverUserId && u.Username.Contains(searchTerm))));
            }

            return await query.CountAsync();
        }
    }
}

