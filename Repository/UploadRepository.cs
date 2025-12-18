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

        public async Task<Upload?> GetByIdWithDetailsAsync(Guid uploadId)
        {
            return await _context.Uploads
                .Include(u => u.Category)
                .Include(u => u.User)
                .Include(u => u.Staff)
                .FirstOrDefaultAsync(u => u.Id == uploadId);
        }

        public async Task<List<Upload>> GetByStatusAsync(string status)
        {
            return await _context.Uploads
                .Include(u => u.Category)
                .Include(u => u.User)
                .Include(u => u.Staff)
                .Where(u => u.Status == status)
                .ToListAsync();
        }

        public async Task<List<Upload>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Uploads
                .Include(u => u.Category)
                .Include(u => u.User)
                .Include(u => u.Staff)
                .Where(u => u.Userid == userId)
                .ToListAsync();
        }

        public async Task<List<Upload>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Uploads
                .Include(u => u.Category)
                .Include(u => u.User)
                .Include(u => u.Staff)
                .Where(u => u.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid uploadId)
        {
            return await _context.Uploads.AnyAsync(u => u.Id == uploadId);
        }

        public async Task<List<Upload>> GetAllWithDetailsAsync()
        {
            return await _context.Uploads
                .Include(u => u.Category)
                .Include(u => u.User)
                .Include(u => u.Staff)
                .ToListAsync();
        }

        public async Task<List<Upload>> SearchUploadsAsync(
            string? status = null,
            Guid? userId = null,
            Guid? categoryId = null,
            Guid? staffId = null,
            string? type = null,
            string? searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Uploads
                .Include(u => u.Category)
                .Include(u => u.User)
                .Include(u => u.Staff)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (userId.HasValue)
                query = query.Where(u => u.Userid == userId.Value);

            if (categoryId.HasValue)
                query = query.Where(u => u.CategoryId == categoryId.Value);

            if (staffId.HasValue)
                query = query.Where(u => u.Staffid == staffId.Value);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(u => u.Type == type);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(u =>
                    u.Name.Contains(searchTerm) ||
                    (u.Description != null && u.Description.Contains(searchTerm)));

            if (fromDate.HasValue)
                query = query.Where(u => u.LostDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(u => u.LostDate <= toDate.Value);

            // Pagination
            query = query
                .OrderByDescending(u => u.DateCreate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountUploadsAsync(
            string? status = null,
            Guid? userId = null,
            Guid? categoryId = null,
            Guid? staffId = null,
            string? type = null,
            string? searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.Uploads.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (userId.HasValue)
                query = query.Where(u => u.Userid == userId.Value);

            if (categoryId.HasValue)
                query = query.Where(u => u.CategoryId == categoryId.Value);

            if (staffId.HasValue)
                query = query.Where(u => u.Staffid == staffId.Value);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(u => u.Type == type);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(u =>
                    u.Name.Contains(searchTerm) ||
                    (u.Description != null && u.Description.Contains(searchTerm)));

            if (fromDate.HasValue)
                query = query.Where(u => u.LostDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(u => u.LostDate <= toDate.Value);

            return await query.CountAsync();
        }
    }
}

