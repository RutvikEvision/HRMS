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

        public async Task CreateAsync(Organization organization, OrganizationUser membership)
        {
            _context.Organizations.Add(organization);
            _context.OrganizationUsers.Add(membership);

            await _context.SaveChangesAsync();
        }

        public async Task<List<OrganizationUser>> GetMembershipsByUserIdAsync(Guid userId)
        {
            return await _context.OrganizationUsers
                .AsNoTracking()
                .Include(m => m.Organization)
                .Where(m => m.UserId == userId && m.IsActive)
                .ToListAsync();
        }
    }
}
