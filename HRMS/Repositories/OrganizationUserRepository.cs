using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.Data.Enums;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class OrganizationUserRepository : IOrganizationUserRepository
    {
        private readonly HRMSDbContext _context;

        public OrganizationUserRepository(HRMSDbContext context)
        {
            _context = context;
        }

        public Task<bool> IsActiveAdminAsync(Guid organizationId, Guid userId)
        {
            return _context.OrganizationUsers.AnyAsync(m =>
                m.OrganizationId == organizationId &&
                m.UserId == userId &&
                m.Role == OrganizationRole.Admin &&
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

        public Task<List<OrganizationUser>> GetActiveHrMembershipsCreatedByAdminAsync(Guid organizationId, Guid adminUserId)
        {
            return _context.OrganizationUsers
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m =>
                    m.OrganizationId == organizationId &&
                    m.Role == OrganizationRole.HR &&
                    m.CreatedByUserId == adminUserId &&
                    m.IsActive)
                .OrderBy(m => m.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<OrganizationUser>> GetActiveHrMembershipsByOrganizationAsync(Guid organizationId)
        {
            return await _context.OrganizationUsers
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m =>
                    m.OrganizationId == organizationId &&
                    m.Role == OrganizationRole.HR &&
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
