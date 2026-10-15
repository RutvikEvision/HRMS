using HRMS.Data.Entities;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class HrUserService : IHrUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizationUserRepository _membershipRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public HrUserService(UserManager<ApplicationUser> userManager, IOrganizationUserRepository membershipRepository, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _membershipRepository = membershipRepository;
            _roleManager = roleManager;
        }

        public async Task<(bool Success, string Message, Guid? UserId)> CreateAsync(
            Guid adminUserId, 
            Guid organizationId, 
            string email, 
            string password)
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");

            if (adminRole is null)
            {
                throw new InvalidOperationException("Admin role not found.");
            }

            var isAdmin = await _membershipRepository.IsActiveAdminAsync(organizationId, adminUserId, adminRole.Id);

            if (!isAdmin)
            {
                return (false, "You are not an active Admin of this organization.", null);
            }

            var hrRole = await _roleManager.FindByNameAsync("HR");

            if (hrRole is null)
            {
                throw new InvalidOperationException("HR role not found.");
            }

            email = email.Trim();

            var user = await _userManager.FindByEmailAsync(email);
            var isNewUser = user is null;

            if (isNewUser)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = email,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(e => e.Description));

                    return (false, errors, null);
                }
            }
            try
            {
                //check that in current organization user already exists or what?
                var membership = await _membershipRepository.GetMembershipAsync(organizationId, user!.Id);

                // user exists
                if (membership is not null)
                {
                    // user also active then we already have that user in organization
                    if (membership.IsActive)
                    {
                        if (isNewUser)
                        {
                            await _userManager.DeleteAsync(user);
                        }

                        return (false,
                            "This user is already a member of this organization.",
                            null);
                    }

                    // if membership already exists and in-active but the role is not HR
                    if (membership.RoleId != hrRole.Id)
                    {
                        if (isNewUser)
                        {
                            await _userManager.DeleteAsync(user);
                        }

                        return (false,
                            "This user has an inactive non-HR membership. " +
                            "Handle that membership separately before assigning HR.",
                            null);
                    }

                    //otherwise we active that user
                    membership.CreatedByUserId = adminUserId;
                    membership.CreatedAtUtc = DateTime.UtcNow;
                    membership.IsActive = true;

                    await _membershipRepository.UpdateAsync(membership);
                }                
                // user not exists in organization so create HR
                else
                {
                    membership = new OrganizationUser
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organizationId,
                        UserId = user.Id,
                        RoleId = Guid.Parse(hrRole.Id.ToString()),
                        CreatedByUserId = adminUserId,
                        CreatedAtUtc = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _membershipRepository.AddAsync(membership);
                }

                return (true, "HR user added successfully.", user.Id);
            }
            catch
            {
                if(isNewUser)
                {
                    _ = await _userManager.DeleteAsync(user!);
                }
                throw;
            }
        }

        public async Task<(bool IsAuthorized, bool Found)> DeactivateAsync(Guid adminUserId, Guid organizationId, Guid hrUserId)
        {
            var hrRole = await _roleManager.FindByNameAsync("HR");

            if (hrRole is null)
            {
                throw new InvalidOperationException("HR role not found.");
            }

            var adminRole = await _roleManager.FindByNameAsync("Admin");

            if (adminRole is null)
            {
                throw new InvalidOperationException("Admin role not found.");
            }

            var isAdmin = await _membershipRepository.IsActiveAdminAsync(organizationId, adminUserId, adminRole.Id);

            if (!isAdmin)
            {
                return (false, false);
            }

            var membership = await _membershipRepository.GetActiveMembershipAsync(
                organizationId,
                hrUserId);

            if (membership is null || !membership.IsActive || membership.RoleId != hrRole.Id)
            {
                return (true, false);
            }

            membership.IsActive = false;

            await _membershipRepository.UpdateAsync(membership);

            return (true, true);
        }
    }
}
