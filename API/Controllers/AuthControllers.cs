using BusinessObjectLayer.IService;
using BusinessObjectLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// Authentication API Controller - Handles user login, registration, and profile
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// POST /api/auth/login - User login
        /// </summary>
        /// <param name="request">Login credentials (username and password)</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">Login successful - returns JWT token</response>
        /// <response code="400">Invalid request format</response>
        /// <response code="401">Invalid credentials or inactive account</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Login request received for username: {Username}", request.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login validation failed for username: {Username}", request.Username);
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                throw new ValidationException(errors);
            }

            var result = await _authService.LoginAsync(request);
            
            _logger.LogInformation("Login successful for username: {Username}", request.Username);
            
            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful"));
        }

        /// <summary>
        /// POST /api/auth/register - User registration
        /// </summary>
        /// <param name="request">User registration information</param>
        /// <returns>JWT token and newly created user information</returns>
        /// <response code="201">Registration successful - user created</response>
        /// <response code="400">Invalid request format or validation error</response>
        /// <response code="409">Username or email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation("Registration request received for username: {Username}", request.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Registration validation failed for username: {Username}", request.Username);
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                throw new ValidationException(errors);
            }

            var result = await _authService.RegisterAsync(request);
            
            _logger.LogInformation("Registration successful for username: {Username}, UserId: {UserId}", 
                request.Username, result.UserId);

            // REST standard: 201 Created with Location header
            return CreatedAtAction(
                nameof(GetCurrentUser),
                new { },
                ApiResponse<AuthResponseDto>.Ok(result, "Registration successful")
            );
        }

        /// <summary>
        /// POST /api/auth/logout - User logout
        /// </summary>
        /// <returns>Logout confirmation message</returns>
        /// <response code="200">Logout successful</response>
        /// <response code="401">User not authenticated</response>
        /// <remarks>
        /// JWT is stateless, so logout is handled client-side by removing the token.
        /// This endpoint can be used for logging or token blacklisting if needed.
        /// </remarks>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        public IActionResult Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            
            _logger.LogInformation("User logged out - UserId: {UserId}, Username: {Username}", userId, username);

            return Ok(ApiResponse<object>.Ok(
                null, 
                "Logout successful. Please remove the token from client storage."
            ));
        }

        /// <summary>
        /// GET /api/auth/me - Get current authenticated user profile
        /// </summary>
        /// <returns>Current user profile information</returns>
        /// <response code="200">User profile retrieved successfully</response>
        /// <response code="401">User not authenticated or invalid token</response>
        /// <response code="404">User not found</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user token - cannot extract UserId from claims");
                throw new UnauthorizedException("Invalid user token");
            }

            _logger.LogDebug("Retrieving profile for UserId: {UserId}", userId);

            var user = await _authService.GetCurrentUserAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                throw new NotFoundException("User", userId.ToString());
            }

            return Ok(ApiResponse<UserDto>.Ok(user, "User profile retrieved successfully"));
        }
    }
}
