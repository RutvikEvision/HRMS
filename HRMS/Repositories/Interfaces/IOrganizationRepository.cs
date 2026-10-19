using HRMS.Data.Entities;

namespace HRMS.Repositories.Interfaces
{
    public interface IOrganizationRepository
    {
        Task CreateWithAdminAsync(Organization organization, OrganizationUser membership);
    }
}
