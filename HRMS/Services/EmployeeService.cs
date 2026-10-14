using HRMS.Data.Entities;
using HRMS.Data.Enums;
using HRMS.DTOs;
using HRMS.Repositories;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrganizationUserRepository _membershipRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IOrganizationUserRepository membershipRepository)
        {
            _employeeRepository = employeeRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<(bool IsAuthorized, Employee? Employee)> CreateAsync(Guid hrUserId, Guid organizationId, CreateEmployeeRequest request)
        {
            var membership = await _membershipRepository.GetActiveMembershipAsync(organizationId, hrUserId);

            if (membership is null || !membership.IsActive || membership.Role != OrganizationRole.HR)
            {
                return (false, null);
            }

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                CreatedByUserId = hrUserId,
                UserId = null,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) 
                    ? null 
                    : request.PhoneNumber.Trim(),
                Address = request.Address.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _employeeRepository.AddAsync(employee);

            return (true, employee);
        }


        public async Task<(bool IsAuthorized, bool Found)> UpdateAsync(
            Guid hrUserId,
            Guid organizationId,
            Guid employeeId,
            UpdateEmployeeRequest request)
        {
            var membership = await _membershipRepository.GetActiveMembershipAsync(organizationId, hrUserId);

            if (membership is null ||
                !membership.IsActive ||
                membership.Role != OrganizationRole.HR)
            {
                return (false, false);
            }

            var employee = await _employeeRepository.GetEmployeeCreatedByHrAsync(
                organizationId,
                hrUserId,
                employeeId);

            if (employee is null)
            {
                return (true, false);
            }

            employee.FirstName = request.FirstName.Trim();
            employee.LastName = request.LastName.Trim();
            employee.Email = request.Email.Trim();
            employee.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim();
            employee.Address = request.Address.Trim();
            employee.UpdatedAtUtc = DateTime.UtcNow;

            await _employeeRepository.UpdateAsync(employee);

            return (true, true);
        }


        public async Task<(bool IsAuthorized, bool Found)> DeleteAsync(Guid organizationId, Guid hrUserId, Guid employeeId)
        {
            var membership = await _membershipRepository.GetActiveMembershipAsync(organizationId, hrUserId);

            if (membership is null ||!membership.IsActive || membership.Role != OrganizationRole.HR)
            {
                return (false, false);
            }

            var employee = await _employeeRepository.GetEmployeeCreatedByHrAsync(organizationId, hrUserId, employeeId);

            if (employee is null)
            {
                return (true, false);
            }

            await _employeeRepository.DeleteAsync(employee);

            return (true, true);
        }

        public async Task<(bool IsAuthorized, EmployeeDetailResponse? Details)> GetDetailsAsync(Guid currentUserId, Guid organizationId, Guid employeeId)
        {
            var membership = await _membershipRepository.GetActiveMembershipAsync(organizationId, currentUserId);

            if (membership is null || !membership.IsActive)
            {
                return (false, null);
            }

            if (membership.Role == OrganizationRole.Admin)
            {
                var adminDetails = await _employeeRepository.GetDetailsAsync(organizationId, employeeId);

                return adminDetails is null ? (true, null) : (true, adminDetails);
            }

            if (membership.Role == OrganizationRole.HR)
            {
                var employee = await _employeeRepository.GetEmployeeCreatedByHrAsync(organizationId, currentUserId, employeeId);

                if (employee is null)
                {
                    return (true, null);
                }

                var details = await _employeeRepository.GetDetailsAsync(organizationId, employeeId);

                return (true, details);
            }

            return (false, null);
        }
    }
}
