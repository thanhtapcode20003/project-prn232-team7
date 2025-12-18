using BusinessObjectLayer.Exceptions;
using DataAccessLayer.DbContxts;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessObjectLayer.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LostAndFoundDbContext _context;
        public UserService(
            IHttpContextAccessor httpContextAccessor,
            LostAndFoundDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("User not authenticated");
            }

            return Guid.Parse(userIdClaim);
        }
        public User GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new UnauthorizedException("User not found");
            }
            return user;
        }
    }
}
