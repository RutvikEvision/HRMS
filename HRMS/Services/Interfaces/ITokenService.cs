using HRMS.Data.Entities;

namespace HRMS.Services.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAtUtc) CreateAccessToken(ApplicationUser user, Guid? organizationId, string? role);
    }
}
