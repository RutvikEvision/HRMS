using Microsoft.AspNetCore.Identity;

namespace HRMS.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
        public ICollection<OrganizationUser> Memberships { get; set; } = new List<OrganizationUser>();
    }
}
