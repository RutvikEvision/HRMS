using HRMS.Data.Entities;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _repository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public OrganizationService(IOrganizationRepository repository, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _repository = repository;
            _roleManager = roleManager;
        }

        public async Task<Organization> CreateAsync(Guid userId, string name, string address)
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");

            if (adminRole is null)
            {
                throw new InvalidOperationException("Admin role not found");
            }

            var now = DateTime.UtcNow;

            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Address = address.Trim(),
                CreatedAtUtc = now
            };

            var membership = new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organization.Id,
                UserId = userId,
                RoleId = adminRole.Id,
                CreatedByUserId = userId,
                CreatedAtUtc = now,
                IsActive = true
            };

            await _repository.CreateAsync(organization, membership);

            return organization;
        }

        public Task<List<OrganizationUser>> GetMineOrganizationsAsync(Guid userId)
        {
            return _repository.GetMembershipsByUserIdAsync(userId);
        }
    }
}
