using HRMS.DTOs;

namespace HRMS.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool IsAuthorized, OrganizationUsersResponse? Response)> GetOrganizationUsersAsync(Guid currentUserId, Guid organizationId);

    }
}
