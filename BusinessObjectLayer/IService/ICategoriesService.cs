using BusinessObjectLayer.DTOs;
using BusinessObjectLayer.DTOs.Categories;

namespace BusinessObjectLayer.IService
{
    public interface ICategoriesService
    {
        Task<List<CategoriesResponse>> GetAllCate();
        Task<CategoriesResponse> GetCateById(Guid id);
        Task<CategoriesResponse> CreateCate(CategoriesRequest request);
        Task<CategoriesResponse> UpdateCate(Guid id, CategoriesRequest request);
        Task<bool> DeleteCate(Guid id);
        Task<PaginationResult<List<CategoriesResponse>>> GetCatesPaged(CategoriesFillter fillter);
    }
}
