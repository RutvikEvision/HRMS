using HRMS.Data.Entities;
using HRMS.DTOs;

namespace HRMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<(bool IsAuthorized, Employee? Employee)> CreateAsync(Guid hrUserId, Guid organizationId, CreateEmployeeRequest request);
        Task<(bool IsAuthorized, bool Found)> UpdateAsync(Guid hrUserId, Guid organizationId, Guid employeeId, UpdateEmployeeRequest request);
        Task<(bool IsAuthorized, bool Found)> DeleteAsync(Guid organizationId, Guid hrUserId, Guid employeeId);
        Task<(bool IsAuthorized, EmployeeDetailResponse? Details)> GetDetailsAsync(Guid currentUserId, Guid organizationId, Guid employeeId);
    }
}
