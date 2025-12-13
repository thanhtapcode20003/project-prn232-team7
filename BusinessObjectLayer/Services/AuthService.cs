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
using BusinessObjectLayer.Enum;

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
        _logger.LogInformation("Login attempt for username: {Username}", request.Username);

        // Tìm user theo username
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found - {Username}", request.Username);
            throw new UnauthorizedException("Invalid username or password");
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.Password))
        {
            _logger.LogWarning("Login failed: Invalid password for user - {Username}", request.Username);
            throw new UnauthorizedException("Invalid username or password");
        }

        // Check user status
        if (!user.Status.Equals(StatusEnum.ACTIVE.ToString()))
        {
            _logger.LogWarning("Login failed: Account inactive - {Username}", request.Username);
            throw new UnauthorizedException("Account is inactive");
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

        _logger.LogInformation("Login successful for user: {UserId} - {Username}", user.UserId, user.Username);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username!,
            Name = user.FullName ?? user.Name ?? string.Empty,
            Gmail = user.Email,
            Token = token,
            RoleName = user.Role.RoleName,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        _logger.LogInformation("Registration attempt for username: {Username}", request.Username);

        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: Username already exists - {Username}", request.Username);
            throw new ApiException(409, "HB40901", "Username already exists");
        }

        // Check if email already exists
        var existingEmail = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Gmail);

        if (existingEmail != null)
        {
            _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Gmail);
            throw new ApiException(409, "HB40901", "Email already exists");
        }

        // Get default role (Student role)
        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Student");
        if (userRole == null)
        {
            _logger.LogError("Registration failed: Default user role 'Student' not found in database");
            throw new ApiException(500, "HB50001", "Default user role not found");
        }

        // Create new user
        var newUser = new User
        {
            FullName = request.Name,
            Name = request.Name,  // Đồng bộ cả hai field
            Username = request.Username,
            Password = HashPassword(request.Password), // Hash password
            Email = request.Gmail,
            PhoneNumber = request.Phone,
            Address = request.Address,
            RoleId = userRole.RoleId,
            Status = StatusEnum.ACTIVE.ToString() // Active by default
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created successfully: {UserId} - {Username}", newUser.UserId, newUser.Username);

        newUser.Role = userRole;

        var token = GenerateJwtToken(newUser);
        var expiresAt = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

        return new AuthResponseDto
        {
            UserId = newUser.UserId,
            Username = newUser.Username!,
            Name = newUser.FullName ?? newUser.Name ?? string.Empty,
            Gmail = newUser.Email,
            Token = token,
            RoleName = newUser.Role.RoleName,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        _logger.LogInformation("Getting current user info for: {UserId}", userId);

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return null;
        }

        return new UserDto
        {
            Id = user.UserId,
            Name = user.FullName ?? user.Name ?? string.Empty,
            Phone = user.PhoneNumber,
            Gmail = user.Email,
            Address = user.Address,
            Username = user.Username,
            RoleName = user.Role?.RoleName ?? "Unknown",
            Status = user.Status
        };
    }

    private string GenerateJwtToken(User user)
    {
        if (user.Role == null)
        {
            _logger.LogError("Cannot generate JWT token: User.Role is null for UserId: {UserId}", user.UserId);
            throw new InvalidOperationException("User role is not loaded");
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Name ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24")),
            signingCredentials: credentials
        );

        _logger.LogDebug("JWT token generated for user: {UserId}", user.UserId);

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
