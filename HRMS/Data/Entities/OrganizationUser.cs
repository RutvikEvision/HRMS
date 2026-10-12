using HRMS.Data.Enums;

namespace HRMS.Data.Entities
{
    public class OrganizationUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public OrganizationRole Role { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;
        public Organization Organization { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public ApplicationUser? CreatedByUser { get; set; }
    }
}
