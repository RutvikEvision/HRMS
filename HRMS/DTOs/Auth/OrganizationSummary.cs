namespace HRMS.DTOs.Auth
{
    public class OrganizationSummary
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
