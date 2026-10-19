using HRMS.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Data
{
    public class HRMSDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }

        public DbSet<Organization> Organizations => Set<Organization>();

        public DbSet<OrganizationUser> OrganizationUsers => Set<OrganizationUser>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //ApplicationUser for extra fields
            builder.Entity<ApplicationUser>(e =>
            {
                e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
                e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
                e.Property(x => x.Address).HasMaxLength(300);
                e.Property(x => x.CreatedAtUtc).IsRequired();
            });

            // Organization
            builder.Entity<Organization>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(150);
                e.Property(x => x.Address).IsRequired().HasMaxLength(300);
                e.Property(x => x.CreatedAtUtc).IsRequired();

                e.HasMany(x => x.Memberships)
                 .WithOne(x => x.Organization)
                 .HasForeignKey(x => x.OrganizationId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Organization membership
            builder.Entity<OrganizationUser>(e =>
            {
                // One role per user per organization
                e.HasIndex(x => new { x.OrganizationId, x.UserId }).IsUnique();
                e.HasIndex(x => x.UserId);
                e.HasIndex(x => x.CreatedByUserId);

                e.Property(x => x.IsActive).IsRequired();
                e.Property(x => x.CreatedAtUtc).IsRequired();

                e.HasOne(x => x.User)
                 .WithMany(x => x.Memberships)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Role)
                 .WithMany()
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CreatedByUser)
                 .WithMany()
                 .HasForeignKey(x => x.CreatedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
