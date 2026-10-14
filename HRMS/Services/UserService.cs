using HRMS.Data.Enums;
using HRMS.DTOs;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;

namespace HRMS.Services
{
    public class UserService : IUserService
    {
        private readonly IOrganizationUserRepository _membershipRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public UserService(IOrganizationUserRepository membershipRepository, IEmployeeRepository employeeRepository)
        {
            _membershipRepository = membershipRepository;
            _employeeRepository = employeeRepository;
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
            if (membership.Role == OrganizationRole.Admin)
            {
                //Get HR users
                var hrMemberships =
                    await _membershipRepository
                        .GetActiveHrMembershipsByOrganizationAsync(
                            organizationId);

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
            if (membership.Role == OrganizationRole.HR)
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
