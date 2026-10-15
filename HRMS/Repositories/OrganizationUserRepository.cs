using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class OrganizationUserRepository : IOrganizationUserRepository
    {
        private readonly HRMSDbContext _context;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public OrganizationUserRepository(HRMSDbContext context, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<bool> IsActiveAdminAsync(Guid organizationId, Guid userId, Guid adminRoleId)
        {

            return await _context.OrganizationUsers.AnyAsync(m =>
                m.OrganizationId == organizationId &&
                m.UserId == userId &&
                m.RoleId == adminRoleId &&
                m.IsActive
            );
        }

        public Task<OrganizationUser?> GetMembershipAsync(Guid organizationId, Guid userId)
        {
            return _context.OrganizationUsers.FirstOrDefaultAsync(m =>
                m.OrganizationId == organizationId && 
                m.UserId == userId
            );
        }

        public Task<OrganizationUser?> GetActiveMembershipAsync(Guid organizationId, Guid userId)
        {
            return _context.OrganizationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.OrganizationId == organizationId &&
                    m.UserId == userId &&
                    m.IsActive);
        }

        public async Task<List<OrganizationUser>> GetActiveHrMembershipsByOrganizationAsync(Guid organizationId, Guid hrRoleId)
        {
            return await _context.OrganizationUsers
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m =>
                    m.OrganizationId == organizationId &&
                    m.RoleId == hrRoleId &&
                    m.IsActive)
                .OrderBy(m => m.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task AddAsync(OrganizationUser membership) 
        {
            _context.OrganizationUsers.Add(membership);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrganizationUser membership)
        {
            _context.OrganizationUsers.Update(membership);
            await _context.SaveChangesAsync();
        }
    }
}
