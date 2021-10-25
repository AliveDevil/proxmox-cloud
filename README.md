# Proxmox Cloud

## Roadmap
Implementation is against my PVE setup, expect to see, some time in the future, a capable OpenStack Horizon-alike dashboard for PVE.
Non-exhaustive list of features planned, implemented, or on the radar:

- Provide proper project management capabilities (detached from PAM or PVE authentication)
  - Projects (can be created and disposed as required)
  - Groups (Combine users, and assign per project privileges to groups)
  - Users
  - Requires only one pve-user for the Proxmox Cloud-VM with Administrator privileges
- Cluster-wide auto migration/load balancing of VMs
  - LXC not supported [1]
- QEMU management and user provisioning
- OpenStack-like configuration:
  - Nova Flavors
  - Neutron provider, project and user-provisioned networks
    - Features unsupported by OpenStack (upto) Xena: QinQ over 802.1q
  - Glance Images
  - Cinder Volumes
- RHV/oVirt-like global cluster options
  - CPU Model for VMs
- Image management on Ceph RBD
  - This will bypass PVE ISO template management for CephFS, using straight snapshots/clones.
- Storage Management, Ceph only
- Volume management on Ceph RBD
- Image/Template management
  - Using Proxmox Templates is _no_ requirement,  
    if I can work around Proxmox Templates, I will.
- HA for VMs
  - ? Automatic (cloud-wide)
  - ? Manual (configurable by user)
- One-Way migration of already existing cluster resources (VM, SDN Network) to Proxmox Cloud
  - If possible renaming SDN Zones and VNets will happen for easier management in Proxmox Cloud
- VM description containing CommonMark description

Proxmox Cloud won't be a cluster management platform, as the PVE Web GUI is already well-suited for this job.  
This will be a self-service cloud-like platform on top of PVE.

## Help Wanted
Currently I cannot accept any help, as I'm still figuring this out.

Though these parts will be left open, unless I find myself needing these in my deployment:
- SDN-features, except
  - Simple
  - QinQ tunneling over 802.1q
  - VLAN
  - VXLAN
- Storage-management, except
  - CephFS (LXC Root FS)
  - Ceph RBD
- Backup
- Replication
- ACME
- Firewall
- Permissions (unless already implemented by internal identity support)

## License
Licensed under EUPL-1.2, see [LICENSE](LICENSE).

[1]: https://forum.proxmox.com/threads/live-migration-lxc.38682/