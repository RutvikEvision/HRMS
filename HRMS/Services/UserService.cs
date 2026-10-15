using HRMS.DTOs;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class UserService : IUserService
    {
        private readonly IOrganizationUserRepository _membershipRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserService(IOrganizationUserRepository membershipRepository, IEmployeeRepository employeeRepository, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _membershipRepository = membershipRepository;
            _employeeRepository = employeeRepository;
            _roleManager = roleManager;
        }

        public async Task<(bool IsAuthorized, OrganizationUsersResponse? Response)> GetOrganizationUsersAsync(
            Guid currentUserId,
            Guid organizationId)
        {
            //Get Current User's membership
            var membership = await _membershipRepository.GetActiveMembershipAsync(organizationId, currentUserId);

            if (membership is null || !membership.IsActive)
            {
                return (false, null);
            }

            var response = new OrganizationUsersResponse();

            //If Current User's Role Admin
            var adminRole = await _roleManager.FindByNameAsync("Admin");

            if(adminRole is null)
            {
                throw new InvalidOperationException("Admin role not found.");
            }

            if (membership.RoleId == adminRole.Id)
            {
                var hrRole = await _roleManager.FindByNameAsync("HR");

                if (hrRole is null)
                {
                    throw new InvalidOperationException("HR role not found");
                }
                //Get HR users
                var hrMemberships = await _membershipRepository.GetActiveHrMembershipsByOrganizationAsync(organizationId, hrRole.Id);

                //Also get Employee users
                var employees =
                    await _employeeRepository
                        .GetEmployeesByOrganizationAsync(organizationId);

                response.HrUsers = hrMemberships
                    .Select(m => new HrUserResponse
                    {
                        UserId = m.UserId,
                        Email = m.User.Email ?? string.Empty,
                        CreatedAtUtc = m.CreatedAtUtc
                    })
                    .ToList();

                response.Employees = employees
                    .Select(e => new EmployeeResponse
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Email = e.Email,
                        PhoneNumber = e.PhoneNumber,
                        Address = e.Address,
                        CreatedByUserId = e.CreatedByUserId,
                        CreatedAtUtc = e.CreatedAtUtc
                    })
                    .ToList();

                return (true, response);
            }

            //If Current User's Role is HR
            if (membership.RoleId == adminRole.Id)
            {
                //Then only get Employees
                var employees =
                    await _employeeRepository
                        .GetEmployeesCreatedByHrAsync(
                            organizationId,
                            currentUserId);

                response.Employees = employees
                    .Select(e => new EmployeeResponse
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Email = e.Email,
                        PhoneNumber = e.PhoneNumber,
                        Address = e.Address,
                        CreatedByUserId = e.CreatedByUserId,
                        CreatedAtUtc = e.CreatedAtUtc
                    })
                    .ToList();

                return (true, response);
            }

            return (false, null);
        }
    }
}
