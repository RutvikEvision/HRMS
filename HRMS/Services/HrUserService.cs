using HRMS.Data.Entities;
using HRMS.Data.Enums;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class HrUserService : IHrUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizationUserRepository _membershipRepository;

        public HrUserService(UserManager<ApplicationUser> userManager, IOrganizationUserRepository membershipRepository)
        {
            _userManager = userManager;
            _membershipRepository = membershipRepository;
        }

        public async Task<(bool Success, string Message, Guid? UserId)> CreateAsync(
            Guid adminUserId, 
            Guid organizationId, 
            string email, 
            string password)
        {
            var isAdmin = await _membershipRepository.IsActiveAdminAsync(organizationId, adminUserId);

            if (!isAdmin)
            {
                return (false, "You are not an active Admin of this organization.", null);
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
                //check in current organization user exists
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
                    if (membership.Role != OrganizationRole.HR)
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
                        Role = OrganizationRole.HR,
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
                    await _userManager.DeleteAsync(user);
                }
                throw;
            }
        }
    }
}
