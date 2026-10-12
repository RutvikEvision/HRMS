using HRMS.Data.Entities;
using HRMS.Data.Enums;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;

namespace HRMS.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _repository;

        public OrganizationService(IOrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Organization> CreateAsync(Guid userId, string name, string address)
        {
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
                Role = OrganizationRole.Admin,
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
