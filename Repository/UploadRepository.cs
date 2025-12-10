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

        public UploadRepository(LostAndFoundSystemDbContext context) : base(context)
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
    }
}

