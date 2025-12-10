using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.Item;

public class UpdateItemRequestDto
{
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid image URL format")]
    public string? Img { get; set; }

    public int? CategoryId { get; set; }

    public int? Status { get; set; }

    [StringLength(255, ErrorMessage = "Found location cannot exceed 255 characters")]
    public string? FoundLocation { get; set; }

    public int? CurrentLocationId { get; set; }

    public string? Content { get; set; }

    public int? UserId { get; set; }

    public DateTime? FoundDate { get; set; }
}