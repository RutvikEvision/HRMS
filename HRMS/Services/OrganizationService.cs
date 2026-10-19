using HRMS.Common;
using HRMS.Data.Entities;
using HRMS.DTOs.Auth;
using HRMS.DTOs.OrganizationDTOs;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IAuthService _authService;

        public OrganizationService(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager, 
            IOrganizationRepository organizationRepository,
            IMembershipRepository membershipRepository,
            IAuthService authService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _organizationRepository = organizationRepository;
            _membershipRepository = membershipRepository;
            _authService = authService;
        }

        public async Task<AuthResponse> CreateWithAdminAsync(Guid userId, CreateOrganizationRequest request)
        {
            var user = await GetUserAsync(userId);

            var adminRole = await _roleManager.FindByNameAsync("Admin")
                ?? throw new AppException(500, "Admin role is not seeded.");

            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Address = request.Address.Trim()
            };

            var membership = new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organization.Id,
                UserId = userId,
                RoleId = adminRole.Id,
                CreatedByUserId = userId
            };

            await _organizationRepository.CreateWithAdminAsync(organization, membership);

            return await _authService.IssueTokensAsync(user, organization.Id);
        }

        public async Task<List<MineOrganizationResponse>> GetMineOrganizationsAsync(Guid userId)
        {
            var memberships = await _membershipRepository.GetActiveMembershipsAsync(userId);

            return memberships
                .Select(m => new MineOrganizationResponse{
                    Id = m.OrganizationId, 
                    Name = m.Organization.Name, 
                    Role = m.Role.Name! 
                })
                .ToList();
        }

        public async Task<AuthResponse> SelectAsync(Guid userId, Guid organizationId)
        {
            var user = await GetUserAsync(userId);

            var memberships = await _membershipRepository.GetActiveMembershipsAsync(userId);

            if (!memberships.Any(m => m.OrganizationId == organizationId))
                throw new AppException(403, "You are not a member of this organization.");

            return await _authService.IssueTokensAsync(user, organizationId);
        }

        private async Task<ApplicationUser> GetUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null || !user.IsActive)
                throw new AppException(401, "User not found or inactive.");

            return user;
        }

    }
}
