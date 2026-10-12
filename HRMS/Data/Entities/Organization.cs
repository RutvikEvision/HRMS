namespace HRMS.Data.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }

        public ICollection<OrganizationUser> Memberships { get; set; } = new List<OrganizationUser>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>(); 

    }
}
