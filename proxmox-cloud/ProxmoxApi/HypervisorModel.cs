namespace proxmox_cloud.ProxmoxApi
{
    public class HypervisorModel
    {
        public HypervisorModel(ClusterResource resource)
        {
            Disk = resource.Disk;
            Id = resource.Id;
            MaxDisk = resource.MaxDisk;
            MaxMem = resource.MaxMem;
            Mem = resource.Mem;
            Node = resource.Node;
        }

        public HypervisorModel(NodeResponse node)
        {
            Cpu = node.Cpu;
            Disk = node.Disk;
            Id = node.Id;
            Level = node.Level;
            MaxCpu = node.MaxCpu;
            MaxDisk = node.MaxDisk;
            MaxMem = node.MaxMem;
            Mem = node.Mem;
            Node = node.Node;
            SSLFingerprint = node.SSLFingerprint;
            Status = node.Status;
            UpTime = node.UpTime;
        }

        public double Cpu { get; }

        public long Disk { get; }

        public string Id { get; }

        public string Level { get; }

        public int MaxCpu { get; }

        public long MaxDisk { get; }

        public long MaxMem { get; }

        public long Mem { get; }

        public string Node { get; }

        public string SSLFingerprint { get; }

        public NodeStatus Status { get; }

        public int UpTime { get; }
    }
}