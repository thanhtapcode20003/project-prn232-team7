using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class UploadRepository : GenericRepository<Upload>
    {
        public UploadRepository() : base()
        {
        }

        public UploadRepository(LostAndFoundDbContext context) : base(context)
        {
        }

        public async Task<List<Upload>> GetByItemIdAsync(Guid itemId)
        {
            return await _context.Uploads
                .Include(u => u.Item)
                .Where(u => u.ItemId == itemId)
                .ToListAsync();
        }

        public async Task<Upload?> GetByIdWithItemAsync(Guid uploadId)
        {
            return await _context.Uploads
                .Include(u => u.Item)
                .FirstOrDefaultAsync(u => u.UploadId == uploadId);
        }

        public async Task<List<Upload>> GetByStatusAsync(string status)
        {
            return await _context.Uploads
                .Include(u => u.Item)
                .Where(u => u.Status == status)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid uploadId)
        {
            return await _context.Uploads.AnyAsync(u => u.UploadId == uploadId);
        }

        public async Task<bool> ItemExistsAsync(Guid itemId)
        {
            return await _context.Items.AnyAsync(i => i.ItemId == itemId);
        }

        public async Task<List<Upload>> GetAllWithItemAsync()
        {
            return await _context.Uploads
                .Include(u => u.Item)
                .ToListAsync();
        }

        public async Task<List<Upload>> SearchUploadsAsync(
            string? status = null,
            string? statusAccept = null,
            Guid? itemId = null,
            string? searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Uploads
                .Include(u => u.Item)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (!string.IsNullOrEmpty(statusAccept))
                query = query.Where(u => u.StatusAccept == statusAccept);

            if (itemId.HasValue)
                query = query.Where(u => u.ItemId == itemId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(u =>
                    u.Item != null &&
                    u.Item.ItemName.Contains(searchTerm));

            if (fromDate.HasValue)
                query = query.Where(u => u.UploadTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(u => u.UploadTime <= toDate.Value);

            // Pagination
            query = query
                .OrderByDescending(u => u.UploadTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountUploadsAsync(
            string? status = null,
            string? statusAccept = null,
            Guid? itemId = null,
            string? searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.Uploads.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (!string.IsNullOrEmpty(statusAccept))
                query = query.Where(u => u.StatusAccept == statusAccept);

            if (itemId.HasValue)
                query = query.Where(u => u.ItemId == itemId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(u =>
                    u.Item != null &&
                    u.Item.ItemName.Contains(searchTerm));

            if (fromDate.HasValue)
                query = query.Where(u => u.UploadTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(u => u.UploadTime <= toDate.Value);

            return await query.CountAsync();
        }
    }
}

