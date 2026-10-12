using HRMS.Data.Entities;

namespace HRMS.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization> CreateAsync(Guid id, string name, string address);

        Task<List<OrganizationUser>> GetMineOrganizationsAsync(Guid userId);
    }
}
