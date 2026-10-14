using HRMS.Data.Entities;

namespace HRMS.Repositories.Interfaces
{
    public interface IOrganizationUserRepository
    {
        Task<bool> IsActiveAdminAsync(Guid organizationId, Guid userId);

        Task<OrganizationUser?> GetMembershipAsync(Guid organizationId, Guid userId);

        Task<OrganizationUser?> GetActiveMembershipAsync(Guid organizationId, Guid userId);

        Task<List<OrganizationUser>> GetActiveHrMembershipsCreatedByAdminAsync(Guid organizationId, Guid adminUserId);

        Task<List<OrganizationUser>> GetActiveHrMembershipsByOrganizationAsync(Guid organizationId);

        Task AddAsync(OrganizationUser membership);

        Task UpdateAsync(OrganizationUser membership);
    }
}
