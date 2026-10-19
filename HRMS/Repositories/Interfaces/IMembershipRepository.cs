using HRMS.Data.Entities;

namespace HRMS.Repositories.Interfaces
{
    public interface IMembershipRepository
    {
        Task<List<OrganizationUser>> GetActiveMembershipsAsync(Guid userId);
        Task<List<OrganizationUser>> GetByOrganizationAsync(Guid organizationId, Guid? createdByUserId);
        Task<OrganizationUser?> GetAsync(Guid organizationId, Guid userId);
        Task AddAsync(OrganizationUser membership);
        Task SaveAsync();

    }
}
