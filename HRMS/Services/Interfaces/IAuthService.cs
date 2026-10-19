using HRMS.Common;
using HRMS.Data.Entities;
using HRMS.DTOs.Auth;

namespace HRMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshAsync(RefreshRequest request);
        Task LogoutAsync(Guid userId);
        Task<AuthResponse> IssueTokensAsync(ApplicationUser user, Guid? activeOrganizationId);
    }
}
