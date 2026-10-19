namespace HRMS.DTOs.OrganizationDTOs
{
    public class MineOrganizationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
