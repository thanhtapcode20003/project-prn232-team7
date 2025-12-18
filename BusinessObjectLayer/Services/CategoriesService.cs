using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Categories;
using BusinessObjectLayer.Enum;
using BusinessObjectLayer.Exceptions;
using BusinessObjectLayer.IService;
using DataAccessLayer.Models;
using Repository;

namespace BusinessObjectLayer.Services
{
    public class CategoriesService : ICategoriesService
    {

        private readonly CategoriesRepositiry _categoriesRepository;
        public CategoriesService()
        {
            _categoriesRepository = new CategoriesRepositiry();
        }
        public CategoriesService(CategoriesRepositiry categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }
        public async Task<CategoriesResponse> CreateCate(CategoriesRequest request)
        {
            var newCate = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Status = StatusEnum.ACTIVE.ToString(),

            };
            await _categoriesRepository.CreateAsync(newCate);
            return MapToDto(newCate);
        }

        public async Task<bool> DeleteCate(Guid id)
        {
            var cate = await _categoriesRepository.GetByIdAsync(id);
            if (cate == null)
            {
                throw new NotFoundException("Category not found", id.ToString());
            }
            cate.Status = "INACTIVE";
            await _categoriesRepository.UpdateAsync(cate);
            return true;
        }

        public async Task<List<CategoriesResponse>> GetAllCate()
        {
            var listCate = await _categoriesRepository.FindAsync(
                c => c.Status == "ACTIVE"
            );
            return listCate.Select(MapToDto).ToList();
        }

        public async Task<CategoriesResponse> GetCateById(Guid id)
        {
            var cate = await _categoriesRepository.GetByIdAsync(id);
            return MapToDto(cate);
        }

        public async Task<PaginationResult<List<CategoriesResponse>>> GetCatesPaged(CategoriesFillter filter)
        {
            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 10;

            var (items, totalItems) =
                await _categoriesRepository.SearchCategoriesAsync(
                    status: StatusEnum.ACTIVE.ToString(),
                    name: filter.Name,
                    description: filter.Description,
                    page: filter.Page,
                    pageSize: filter.PageSize
                );
            var itemsDto = items.Select(MapToDto).ToList();
            return new PaginationResult<List<CategoriesResponse>>
            {
                Items = itemsDto,
                TotalItems = totalItems,
                PageSize = filter.PageSize,
                CurrentPage = filter.Page,
                TotalPages = (int)Math.Ceiling(
                    totalItems / (double)filter.PageSize)
            };
        }

        public async Task<CategoriesResponse> UpdateCate(Guid id, CategoriesRequest request)
        {
            var cate = await _categoriesRepository.GetByIdAsync(id);
            if (cate == null)
            {
                throw new NotFoundException("Category not found", id.ToString());
            }
            cate.Name = request.Name;
            cate.Description = request.Description;
            cate.Dateupdate = DateTime.UtcNow;
            await _categoriesRepository.UpdateAsync(cate);
            return MapToDto(cate);
        }
        public CategoriesResponse MapToDto(Category category)
        {
            return new CategoriesResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status
            };
        }
    }
}
