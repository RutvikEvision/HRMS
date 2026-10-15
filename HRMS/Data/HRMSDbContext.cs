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

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Organization
            builder.Entity<Organization>(entity =>
            {
                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.HasMany(x => x.Memberships)
                    .WithOne(x => x.Organization)
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Employees)
                    .WithOne(x => x.Organization)
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Organization membership
            builder.Entity<OrganizationUser>(entity =>
            {
                entity.HasIndex(x => new
                {
                    x.OrganizationId,
                    x.UserId
                }).IsUnique();

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.OrganizationMemberships)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Employee
            builder.Entity<Employee>(entity =>
            {
                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(30);

                entity.Property(x => x.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.HasIndex(x => x.OrganizationId);

                entity.HasIndex(x => x.CreatedByUserId);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.CreatedEmployees)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //Refresh Token
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.TokenHash)
                    .IsRequired();

                entity.HasIndex(rt => rt.TokenHash)
                    .IsUnique();

                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
