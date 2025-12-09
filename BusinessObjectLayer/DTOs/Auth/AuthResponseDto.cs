namespace BusinessObjectLayer.IService;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Gmail { get; set; }
    public string Token { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
