using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace proxmox_cloud.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUser>(b =>
            {
                b.HasMany<ProjectUser>().WithOne().HasForeignKey((ProjectUser pu) => pu.UserId)
                    .IsRequired();
            });
            builder.Entity<Project>(b =>
            {
                b.HasMany<ProjectUser>().WithOne().HasForeignKey((ProjectUser pu) => pu.ProjectId)
                    .IsRequired();
            });
            builder.Entity<ProjectUser>(b =>
            {
                b.HasKey(r => new { r.UserId, r.ProjectId });
            });
        }
    }
}