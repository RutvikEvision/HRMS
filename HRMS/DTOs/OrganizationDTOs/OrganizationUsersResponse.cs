using HRMS.DTOs.UsersDTOs;

namespace HRMS.DTOs.OrganizationDTOs
{
    public class OrganizationUsersResponse
    {
        public List<HrUserResponse> HrUsers { get; set; } = [];
        public List<EmployeeResponse> Employees { get; set; } = [];
    }
}
