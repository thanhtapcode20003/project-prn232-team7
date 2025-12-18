namespace BusinessObjectLayer.DTOs.Item
{
    public class ItemFilterDto
    {
        public Guid? UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? LocationId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
