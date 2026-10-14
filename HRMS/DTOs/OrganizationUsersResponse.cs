namespace HRMS.DTOs
{
    public class OrganizationUsersResponse
    {
        public List<HrUserResponse> HrUsers { get; set; } = [];
        public List<EmployeeResponse> Employees { get; set; } = [];
    }
}
