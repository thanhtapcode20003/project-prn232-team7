namespace BusinessObjectLayer.DTOs.Categories
{
    public class CategoriesFillter
    {
        public string? Name { get; set; }

        public string? Description { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
