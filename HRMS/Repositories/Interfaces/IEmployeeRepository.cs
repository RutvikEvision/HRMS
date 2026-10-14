using HRMS.Data.Entities;
using HRMS.DTOs;

namespace HRMS.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetEmployeesCreatedByHrAsync(Guid organizationId, Guid hrUserId);
        Task<Employee?> GetEmployeeCreatedByHrAsync(Guid organizationId, Guid hrUserId, Guid employeeId);
        Task<List<Employee>> GetEmployeesByOrganizationAsync(Guid organizationId);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task<EmployeeDetailResponse?> GetDetailsAsync(Guid organizationId, Guid employeeId);
    }
}
