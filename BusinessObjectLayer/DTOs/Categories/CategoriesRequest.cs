namespace BusinessObjectLayer.DTOs.Categories
{
    public class CategoriesRequest
    {

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Status { get; set; }
    }
}
