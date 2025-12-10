namespace BusinessObjectLayer.DTOs.Item;

public class ItemFilterDto
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? Status { get; set; }
    public int? CurrentLocationId { get; set; }
    public int? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "Date";
    public bool SortDescending { get; set; } = true;
}