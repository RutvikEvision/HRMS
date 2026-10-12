using HRMS.Data.Entities;

namespace HRMS.Repositories.Interfaces
{
    public interface IOrganizationRepository
    {
        Task CreateAsync(Organization organization, OrganizationUser membership);

        Task<List<OrganizationUser>> GetMembershipsByUserIdAsync(Guid userId);
    }
}
