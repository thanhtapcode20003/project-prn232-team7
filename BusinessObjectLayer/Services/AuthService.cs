using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusinessObjectLayer.IService;
using BusinessObjectLayer.Exceptions;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BusinessObjectLayer.Services;

public class AuthService : IAuthService
{
    private readonly LostAndFoundSystemDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        LostAndFoundSystemDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Tìm user theo username
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.Password))
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        // Check user status
        if (user.Status == 0)
        {
            throw new UnauthorizedException("Account is inactive");
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username!,
            Name = user.Name,
            Gmail = user.Gmail,
            Token = token,
            RoleName = user.Role.Name,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
            throw new ApiException(409, "HB40901", "Username already exists");
        }

        // Check if email already exists
        var existingEmail = await _context.Users
            .FirstOrDefaultAsync(u => u.Gmail == request.Gmail);

        if (existingEmail != null)
        {
            throw new ApiException(409, "HB40901", "Email already exists");
        }

        // Get default role (User role, assuming RoleId = 2)
        var userRole = await _context.Roles.FindAsync(2);
        if (userRole == null)
        {
            throw new ApiException(500, "HB50001", "Default user role not found");
        }

        // Create new user
        var newUser = new User
        {
            Name = request.Name,
            Username = request.Username,
            Password = HashPassword(request.Password), // Hash password
            Gmail = request.Gmail,
            Phone = request.Phone,
            Address = request.Address,
            RoleId = userRole.Id,
            Status = 1 // Active by default
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // Load role for response
        await _context.Entry(newUser).Reference(u => u.Role).LoadAsync();

        // Generate JWT token
        var token = GenerateJwtToken(newUser);
        var expiresAt = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

        return new AuthResponseDto
        {
            UserId = newUser.Id,
            Username = newUser.Username!,
            Name = newUser.Name,
            Gmail = newUser.Gmail,
            Token = token,
            RoleName = newUser.Role.Name,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Gmail = user.Gmail,
            Address = user.Address,
            Username = user.Username,
            RoleName = user.Role.Name,
            Status = user.Status
        };
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Gmail ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string inputPassword, string? storedPassword)
    {
        if (string.IsNullOrEmpty(storedPassword))
            return false;

        var hashedInput = HashPassword(inputPassword);
        return hashedInput == storedPassword;
    }
}
