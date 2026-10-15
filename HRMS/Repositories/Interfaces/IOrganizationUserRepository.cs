using HRMS.Data.Entities;

namespace HRMS.Repositories.Interfaces
{
    public interface IOrganizationUserRepository
    {
        Task<bool> IsActiveAdminAsync(Guid organizationId, Guid userId, Guid adminRoleId);

        Task<OrganizationUser?> GetMembershipAsync(Guid organizationId, Guid userId);

        Task<OrganizationUser?> GetActiveMembershipAsync(Guid organizationId, Guid userId);

        Task<List<OrganizationUser>> GetActiveHrMembershipsByOrganizationAsync(Guid organizationId, Guid hrRoleId);

        Task AddAsync(OrganizationUser membership);

        Task UpdateAsync(OrganizationUser membership);
    }
}
