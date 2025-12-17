using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;

namespace Repository
{
    public class CategoriesRepositiry : GenericRepository<Category>
    {
        public CategoriesRepositiry() : base()
        {
        }
        public CategoriesRepositiry(LostAndFoundDbContext context) : base(context)
        {
        }
        public async Task<(List<Category> Items, int TotalItems)> SearchCategoriesAsync(
         string? status,
         string? name,
         string? description,
         int page,
         int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(c => c.Description.Contains(description));
            }
            var totalItems = await Task.FromResult(query.Count());
            var items = await Task.FromResult(query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList());
            return (items, totalItems);
        }
    }
}
