using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// sources:
//  * TimestampMixin / SoftDeleteMixin: https://opendev.org/openstack/oslo.db/src/branch/master/oslo_db/sqlalchemy/models.py
//  * Identity: https://opendev.org/openstack/keystone/src/branch/master/keystone/identity/backends/sql_model.py
//  * Images: https://opendev.org/openstack/glance/src/branch/master/glance/db/sqlalchemy/models.py
//  * Volumes: https://opendev.org/openstack/cinder/src/branch/master/cinder/db/sqlalchemy/models.py
//  * Instances: https://opendev.org/openstack/nova/src/branch/master/nova/db/main/models.py
//  * Instances 2: https://opendev.org/openstack/nova/src/branch/master/nova/db/api/models.py
namespace proxmox_cloud.Microsoft.EntityFrameworkCore
{
    public static class RelationalPropertyBuilderExtensions
    {
        public static PropertyBuilder<T> ValueGeneration<T>(this PropertyBuilder<T> @this, ValueGenerated generated)
        {
            @this.Metadata.ValueGenerated = generated;
            if ((generated & ValueGenerated.OnAddOrUpdate) > 0)
            {
                @this.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
            }
            if ((generated & ~ValueGenerated.OnAdd) > 0)
            {
                @this.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            }
            return @this;
        }
    }
}