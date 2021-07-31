using System;

namespace proxmox_cloud.Data
{
    public class Project
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public Project()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}