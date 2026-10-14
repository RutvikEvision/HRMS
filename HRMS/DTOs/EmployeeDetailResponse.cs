namespace HRMS.DTOs
{
    public class EmployeeDetailResponse
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime EmployeeCreatedAtUtc { get; set; }

        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;

        public Guid CreatedByUserId { get; set; }
        public string HrEmail { get; set; } = string.Empty;
    }
}
