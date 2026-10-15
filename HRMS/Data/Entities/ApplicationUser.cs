using Microsoft.AspNetCore.Identity;

namespace HRMS.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<OrganizationUser> OrganizationMemberships { get; set; } = new List<OrganizationUser>();
        public ICollection<Employee> CreatedEmployees { get; set; } = new List<Employee>();
    }
}
