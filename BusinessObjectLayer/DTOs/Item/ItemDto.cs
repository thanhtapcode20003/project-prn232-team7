namespace BusinessObjectLayer.DTOs.Item;

public class ItemDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Img { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int Status { get; set; }
    public DateTime? Date { get; set; }
    public string? FoundLocation { get; set; }
    public int? CurrentLocationId { get; set; }
    public string? CurrentLocationName { get; set; }
    public string? Content { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime? FoundDate { get; set; }
}