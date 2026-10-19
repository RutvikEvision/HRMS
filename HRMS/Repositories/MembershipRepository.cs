using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly HRMSDbContext _context;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public MembershipRepository(HRMSDbContext context, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<List<OrganizationUser>> GetActiveMembershipsAsync(Guid userId)
        {
            return await _context.OrganizationUsers
                .AsNoTracking()
                .Include(m => m.Organization)
                .Include(x => x.Role)
                .Where(x => x.UserId == userId && x.IsActive)
                .OrderBy(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<OrganizationUser>> GetByOrganizationAsync(Guid organizationId, Guid? createdByUserId)
        {
            return await _context.OrganizationUsers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Role)
                .Where(x => x.OrganizationId == organizationId && x.IsActive
                            && (createdByUserId == null || x.CreatedByUserId == createdByUserId))
                .OrderBy(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        
        public Task<OrganizationUser?> GetAsync(Guid organizationId, Guid userId)
        {
            return _context.OrganizationUsers
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId);
        }

        public async Task AddAsync(OrganizationUser membership)
        {
            await _context.OrganizationUsers.AddAsync(membership);
        }

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
