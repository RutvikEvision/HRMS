using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly HRMSDbContext _context;

        public OrganizationRepository(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task CreateWithAdminAsync(Organization organization, OrganizationUser membership)
        {
            _context.Organizations.Add(organization);
            _context.OrganizationUsers.Add(membership);

            await _context.SaveChangesAsync();
        }
    }
}
