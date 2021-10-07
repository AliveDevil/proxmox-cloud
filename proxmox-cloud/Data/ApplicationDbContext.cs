using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Data
{
    [StructLayout(LayoutKind.Auto)]
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Flavor> Flavors { get; set; }

        public virtual DbSet<FlavorExtraSpec> FlavorExtraSpecs { get; set; }

        public virtual DbSet<FlavorProject> FlavorProjects { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<ImageLocation> ImageLocations { get; set; }

        public virtual DbSet<ImageProperty> ImageProperties { get; set; }

        public virtual DbSet<Instance> Instances { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

        public virtual DbSet<Volume> Volumes { get; set; }

        public virtual DbSet<VolumeAttachment> VolumeAttachment { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                foreach (var property in entry.Properties)
                {
                    if (!(property.Metadata.ClrType == typeof(DateTime) || property.Metadata.ClrType == typeof(DateTime?)))
                    {
                        continue;
                    }

                    if ((property.Metadata.ValueGenerated & ValueGenerated.OnAdd) > 0 && entry.State == EntityState.Added)
                    {
                        property.CurrentValue = now;
                    }
                    if ((property.Metadata.ValueGenerated & (ValueGenerated.OnUpdate | ValueGenerated.OnUpdateSometimes)) > 0 && entry.State == EntityState.Modified)
                    {
                        property.CurrentValue = now;
                    }
                }
            }
        }
    }
}