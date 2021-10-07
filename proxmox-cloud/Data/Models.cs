using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using proxmox_cloud.Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DeleteBehavior;
using static System.Runtime.InteropServices.LayoutKind;

// sources:
//  * TimestampMixin / SoftDeleteMixin: https://opendev.org/openstack/oslo.db/src/branch/master/oslo_db/sqlalchemy/models.py
//  * Identity: https://opendev.org/openstack/keystone/src/branch/master/keystone/identity/backends/sql_model.py
//  * Images: https://opendev.org/openstack/glance/src/branch/master/glance/db/sqlalchemy/models.py
//  * Volumes: https://opendev.org/openstack/cinder/src/branch/master/cinder/db/sqlalchemy/models.py
//  * Instances: https://opendev.org/openstack/nova/src/branch/master/nova/db/main/models.py
//  * Instances 2: https://opendev.org/openstack/nova/src/branch/master/nova/db/api/models.py
namespace proxmox_cloud.Data
{
    public abstract class ConfiguredType<T> where T : ConfiguredType<T>
    {
        public abstract class TypeConfiguration : IEntityTypeConfiguration<T>
        {
            public abstract void Configure(EntityTypeBuilder<T> builder);
        }
    }

    [StructLayout(Auto)]
    public class Flavor : ConfiguredType<Flavor>
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        public virtual string Name { get; set; }

        public virtual int MemoryMb { get; set; }

        public virtual int vCPUs { get; set; }

        public virtual bool Disabled { get; set; } = false;

        public virtual bool IsPublic { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime CreatedAt { get; }

        public virtual DateTime UpdatedAt { get; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<Flavor> builder)
            {
                builder.Property(x => x.Name).IsRequired();

                builder.Property(x => x.CreatedAt)
                    .ValueGeneration(ValueGenerated.OnAdd);
                builder.Property(x => x.UpdatedAt)
                    .ValueGeneration(ValueGenerated.OnAddOrUpdate);

                builder.HasIndex(x => x.Name).IsUnique();
            }
        }
    }

    [StructLayout(Auto)]
    public class FlavorExtraSpec : ConfiguredType<FlavorExtraSpec>
    {
        public virtual Flavor Flavor { get; set; }

        public virtual string Key { get; set; }

        public virtual string Value { get; set; }

        public virtual DateTime CreatedAt { get; }

        public virtual DateTime UpdatedAt { get; }

        public virtual DateTime? DeletedAt { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<FlavorExtraSpec> builder)
            {
                builder.HasNoKey();

                builder.Property(x => x.Key).IsRequired();
                builder.Property(x => x.CreatedAt)
                    .ValueGeneration(ValueGenerated.OnAdd);
                builder.Property(x => x.UpdatedAt)
                    .ValueGeneration(ValueGenerated.OnAddOrUpdate);

                builder.HasOne(x => x.Flavor).WithMany().HasForeignKey("FlavorId")
                    .IsRequired()
                    .OnDelete(Cascade);

                builder.HasIndex("Key", "FlavorId").IsUnique();
            }
        }
    }

    [StructLayout(Auto)]
    public class FlavorProject : ConfiguredType<FlavorProject>
    {
        public Flavor Flavor { get; set; }

        public Project Project { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<FlavorProject> builder)
            {
                builder.HasNoKey();

                builder.HasOne(x => x.Flavor).WithMany().HasForeignKey("FlavorId")
                    .IsRequired()
                    .OnDelete(Cascade);
                builder.HasOne(x => x.Project).WithMany().HasForeignKey("ProjectId")
                    .IsRequired()
                    .OnDelete(Cascade);

                builder.HasIndex("FlavorId", "ProjectId").IsUnique();
            }
        }
    }

    [StructLayout(Auto)]
    public class Image : ConfiguredType<Image>
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        public virtual string Name { get; set; }

        // disk_format
        // container_format
        public virtual long Size { get; set; }

        public virtual long VirtualSize { get; set; }

        public virtual Visibilities Visibility { get; set; } = Visibilities.shared;

        // checksum
        // os_hash_algo

        public virtual int MinDisk { get; set; }

        public virtual int MinRam { get; set; }

        public virtual IdentityUser Owner { get; set; }

        public virtual bool Protected { get; set; }

        public virtual DateTime CreatedAt { get; }

        public virtual DateTime UpdatedAt { get; }

        public virtual DateTime? DeletedAt { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<Image> builder)
            {
                builder.Property(x => x.CreatedAt)
                    .ValueGeneration(ValueGenerated.OnAdd);
                builder.Property(x => x.UpdatedAt)
                    .ValueGeneration(ValueGenerated.OnAddOrUpdate);

                builder.HasOne(x => x.Owner)
                    .WithMany()
                    .HasForeignKey("OwnerId")
                    .OnDelete(Restrict)
                    .IsRequired();

                builder.HasIndex(x => x.Name)
                    .IsUnique();
            }
        }

        public enum Visibilities
        {
            @private,
            @public,
            shared,
            community
        }
    }

    [StructLayout(Auto)]
    public class ImageLocation : ConfiguredType<ImageLocation>
    {
        public virtual Image Image { get; set; }

        public virtual string Value { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<ImageLocation> builder)
            {
                builder.HasNoKey();

                builder.HasOne(x => x.Image).WithOne()
                    .HasForeignKey<ImageLocation>("ImageId")
                    .OnDelete(Restrict)
                    .IsRequired();
            }
        }
    }

    [StructLayout(Auto)]
    public class ImageProperty : ConfiguredType<ImageProperty>
    {
        public virtual Image Image { get; set; }

        public virtual string Name { get; set; }

        public virtual string Value { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<ImageProperty> builder)
            {
                builder.HasNoKey();

                builder.HasOne(x => x.Image).WithMany()
                    .HasForeignKey("ImageId")
                    .OnDelete(Restrict)
                    .IsRequired();

                builder.HasIndex("ImageId", "Name").IsUnique();
            }
        }
    }

    [StructLayout(Auto)]
    public class Instance : ConfiguredType<Instance>
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        public virtual IdentityUser User { get; set; }

        public virtual Project Project { get; set; }

        public virtual Image Image { get; set; }

        public virtual int MemoryMb { get; set; }

        public virtual int VCPU { get; set; }

        public virtual int RootGB { get; set; }

        public virtual Flavor Flavor { get; set; }

        public virtual DateTime LaunchedAt { get; set; }

        public virtual DateTime TerminatedAt { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string DisplayDescription { get; set; }

        public virtual DateTime CreatedAt { get; }

        public virtual DateTime UpdatedAt { get; }

        public virtual DateTime? DeletedAt { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<Instance> builder)
            {
                builder.Property(x => x.CreatedAt)
                    .ValueGeneration(ValueGenerated.OnAdd);
                builder.Property(x => x.UpdatedAt)
                    .ValueGeneration(ValueGenerated.OnAddOrUpdate);

                builder.HasOne(x => x.User).WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(Restrict)
                    .IsRequired();
                builder.HasOne(x => x.Project).WithMany()
                    .HasForeignKey("ProjectId")
                    .OnDelete(Restrict)
                    .IsRequired();
                builder.HasOne(x => x.Image).WithMany()
                    .HasForeignKey("ImageId")
                    .OnDelete(Restrict)
                    .IsRequired();
                builder.HasOne(x => x.Flavor).WithMany()
                    .HasForeignKey("FlavorId")
                    .OnDelete(Restrict)
                    .IsRequired();
            }
        }
    }

    public partial class Network
    {
        public virtual DateTime CreatedAt { get; }

        public virtual DateTime? DeletedAt { get; set; }

        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Id"), Required]
        public virtual IdentityUser Owner { get; set; }

        [ForeignKey("Id"), Required]
        public virtual Project Project { get; set; }

        public virtual DateTime UpdatedAt { get; }
    }

    [StructLayout(Auto)]
    public class Project : ConfiguredType<Project>
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual Project Parent { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<Project> builder)
            {
                builder.HasOne(x => x.Parent).WithMany()
                    .HasForeignKey("ParentId")
                    .OnDelete(Restrict);

                builder.HasIndex(x => x.Name).IsUnique();
            }
        }
    }

    [StructLayout(Auto)]
    public class ProjectUser : ConfiguredType<ProjectUser>
    {
        public virtual Project Project { get; set; }

        public virtual IdentityUser User { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<ProjectUser> builder)
            {
                builder.HasNoKey();

                builder.HasOne(x => x.Project).WithMany()
                    .HasForeignKey("ParentId")
                    .OnDelete(Restrict)
                    .IsRequired();
                builder.HasOne(x => x.User).WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(Restrict)
                    .IsRequired();

                builder.HasIndex("ParentId", "UserId").IsUnique();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TimestampAttribute : Attribute { }

    [StructLayout(Auto)]
    public class Volume : ConfiguredType<Volume>
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        public virtual IdentityUser User { get; set; }

        public virtual Project Project { get; set; }

        public virtual int Size { get; set; }

        public virtual DateTime ScheduledAt { get; set; }

        public virtual DateTime LaunchedAt { get; set; }

        public virtual DateTime TerminatedAt { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string DisplayDescription { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<Volume> builder)
            {
                builder.HasOne(x => x.User).WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(Restrict)
                    .IsRequired();
                builder.HasOne(x => x.Project).WithMany()
                    .HasForeignKey("ProjctId")
                    .OnDelete(Restrict)
                    .IsRequired();
            }
        }
    }

    [StructLayout(Auto)]
    public class VolumeAttachment : ConfiguredType<VolumeAttachment>
    {
        public virtual Volume Volume { get; set; }

        public virtual Instance Instance { get; set; }

        public sealed class Configuration : TypeConfiguration
        {
            public override void Configure(EntityTypeBuilder<VolumeAttachment> builder)
            {
                builder.HasNoKey();

                builder.HasOne(x => x.Volume).WithMany()
                    .HasForeignKey("VolumeId")
                    .OnDelete(Restrict)
                    .IsRequired();
                builder.HasOne(x => x.Instance).WithMany()
                    .HasForeignKey("InstanceId")
                    .OnDelete(Restrict)
                    .IsRequired();
            }
        }
    }
}