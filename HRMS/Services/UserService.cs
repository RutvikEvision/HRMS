using HRMS.Common;
using HRMS.Data.Entities;
using HRMS.DTOs;
using HRMS.DTOs.UsersDTOs;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMembershipRepository _membershipRepository;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMembershipRepository membershipRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _membershipRepository = membershipRepository;
        }

        
        public async Task<UserResponse> CreateAsync(CurrentUser me, CreateUserRequest request)
        {
            var roleName = me.Role == "Admin" ? "HR" : "Employee";

            var role = await _roleManager.FindByNameAsync(roleName)
                ?? throw new AppException(500, $"{roleName} role is not seeded.");

            var user = await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user is null)
            {
                var hasPassword = !string.IsNullOrWhiteSpace(request.Password);

                if (roleName == "HR" && !hasPassword)
                    throw new AppException(400, "Password is required for a new HR user.");

                user = new ApplicationUser
                {
                    UserName = request.Email.Trim(),
                    Email = request.Email.Trim(),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    PhoneNumber = Clean(request.PhoneNumber),
                    CreatedAtUtc = DateTime.UtcNow,
                    Address = Clean(request.Address)
                };

                var result = hasPassword
                    ? await _userManager.CreateAsync(user, request.Password!)
                    : await _userManager.CreateAsync(user);   // employee without login

                if (!result.Succeeded)
                    throw new AppException(400, string.Join(" ", result.Errors.Select(e => e.Description)));
            }
            else if (!user.IsActive)
            {
                throw new AppException(400, "This user account is disabled.");
            }
            // User already exists so check the membership.

            var membership = await _membershipRepository.GetAsync(me.OrganizationId, user.Id);

            if (membership is { IsActive: true })
                throw new AppException(409, "This user already belongs to your organization.");

            if (membership is null)
            {
                await _membershipRepository.AddAsync(new OrganizationUser
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = me.OrganizationId,
                    UserId = user.Id,
                    RoleId = role.Id,
                    CreatedByUserId = me.UserId
                });
            }
            else   // user exists but inactivated / removed.
            {
                membership.IsActive = true;
                membership.RoleId = role.Id;
                membership.CreatedByUserId = me.UserId;
            }

            await _membershipRepository.SaveAsync();
            return ToResponse(user, roleName);
        }

        public async Task<List<UserResponse>> GetAllAsync(CurrentUser me)
        {
            Guid? createdBy = me.Role == "HR" ? me.UserId : null;

            var list = await _membershipRepository.GetByOrganizationAsync(me.OrganizationId, createdBy);

            return list.Select(m => ToResponse(m.User, m.Role.Name!)).ToList();
        }

        public async Task<UserResponse> UpdateAsync(CurrentUser me, Guid userId, UpdateUserRequest request)
        {
            var membership = await GetManageableAsync(me, userId);

            membership.User.FirstName = request.FirstName.Trim();
            membership.User.LastName = request.LastName.Trim();
            membership.User.PhoneNumber = Clean(request.PhoneNumber);
            membership.User.Address = Clean(request.Address);

            await _membershipRepository.SaveAsync();
            return ToResponse(membership.User, membership.Role.Name!);
        }

        public async Task DeleteAsync(CurrentUser me, Guid userId)
        {
            var membership = await GetManageableAsync(me, userId);

            membership.IsActive = false;   // inactivate/remove from this org only...
            await _membershipRepository.SaveAsync();
        }

        private async Task<OrganizationUser> GetManageableAsync(CurrentUser me, Guid userId)
        {
            var membership = await _membershipRepository.GetAsync(me.OrganizationId, userId);

            if (membership is null || !membership.IsActive)
                throw new AppException(404, "User not found.");

            var allowed = me.Role == "Admin"
                ? membership.Role.Name == "HR"
                : membership.Role.Name == "Employee" && membership.CreatedByUserId == me.UserId;

            if (!allowed)
                throw new AppException(403, "You are not allowed to manage this user.");

            return membership;
        }

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static UserResponse ToResponse(ApplicationUser u, string role)
        {
            return new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                Role = role

            };
        }
    }
}
