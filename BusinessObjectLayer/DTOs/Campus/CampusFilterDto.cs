namespace BusinessObjectLayer.DTOs.Campus
{
    public class CampusFilterDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
