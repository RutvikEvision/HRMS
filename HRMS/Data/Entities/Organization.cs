namespace HRMS.Data.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<OrganizationUser> Memberships { get; set; } = new List<OrganizationUser>();

    }
}
