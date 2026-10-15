
using Microsoft.AspNetCore.Identity;

namespace HRMS.Data.Entities
{
    public class OrganizationUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid RoleId { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;
        public Organization Organization { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public IdentityRole<Guid> Role { get; set; } = null!;
        public ApplicationUser? CreatedByUser { get; set; }
    }
}
