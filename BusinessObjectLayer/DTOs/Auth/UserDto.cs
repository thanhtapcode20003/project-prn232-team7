namespace BusinessObjectLayer.IService;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Gmail { get; set; }
    public string? Address { get; set; }
    public string? Username { get; set; }
    public string RoleName { get; set; } = null!;
    public string Status { get; set; }
}
