using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;

namespace BusinessObjectLayer.IService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<UserDto?> GetCurrentUserAsync(Guid userId);

        // Provide the currently authenticated User from the HttpContext
        User GetCurrentUser();
    }
}
