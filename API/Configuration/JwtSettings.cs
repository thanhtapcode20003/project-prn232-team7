namespace API.Configuration;

/// <summary>
/// Strongly-typed JWT configuration settings
/// Bind from appsettings.json via IOptions<JwtSettings>
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key for JWT signing (minimum 256 bits / 32 characters)
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer (typically your API name)
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience (typically your client app name)
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiry time in hours (default: 24 hours)
    /// </summary>
    public int ExpiryHours { get; set; } = 24;

    /// <summary>
    /// Validate that all required settings are configured
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
            throw new InvalidOperationException("JWT Key is not configured");

        if (Key.Length < 32)
            throw new InvalidOperationException("JWT Key must be at least 32 characters long");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT Audience is not configured");

        if (ExpiryHours <= 0)
            throw new InvalidOperationException("JWT ExpiryHours must be greater than 0");
    }
}
