using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace proxmox_cloud.ProxmoxApi
{
    public enum ClusterResourceType { Node, Storage, Pool, Qemu, LXC, OpenVZ, SDN };

    public enum ClusterStatusType { Cluster, Node }

    public enum NodeStatus { Unknown, Offline, Online }

    public enum StorageType { Btrfs, CephFS, CIFS, Dir, GlusterFS, iSCSI, iSCSIDirect, LVM, LVMThin, NFS, PBS, RBD, ZFS, ZFSPool }

    public sealed class ApplianceInfoResponse : JsonResponse { }

    public sealed class ArrayResponse<T>
    {
        public Collection<T> Data { get; init; }
    }

    public sealed class ClusterOptions : JsonResponse
    {
    }

    public sealed class ClusterResource : JsonResponse
    {
        public string Content { get; init; }

        public double CPU { get; init; }

        public long Disk { get; init; }

        public string HAState { get; init; }

        public string Id { get; init; }

        public double MaxCPU { get; init; }

        public long MaxDisk { get; init; }

        public long MaxMem { get; init; }

        public long Mem { get; init; }

        public string Name { get; init; }

        public string Node { get; init; }

        public string PluginType { get; init; }

        public string Pool { get; init; }

        public string Status { get; init; }

        public string Storage { get; init; }

        [JsonPropertyName("level")]
        public string SupportLevel { get; init; }

        public ClusterResourceType Type { get; init; }

        public int Uptime { get; init; }
    }

    public sealed class ClusterStatusResponse : JsonResponse
    {
        public string Id { get; init; }

        public string IP { get; init; }

        public bool Local { get; init; }

        public string Name { get; init; }

        public int NodeId { get; init; }

        public int Nodes { get; init; }

        public bool Online { get; init; }

        public bool Quorate { get; init; }

        [JsonPropertyName("level")]
        public string SupportLevel { get; init; }

        public ClusterStatusType Type { get; init; }

        public int Version { get; init; }
    }

    public abstract class JsonResponse
    {
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; init; }
    }

    public sealed class NodeConfigResponse : JsonResponse { }

    public sealed class NodeDNSResponse : JsonResponse { }

    public sealed class NodeResponse : JsonResponse
    {
        public double Cpu { get; init; }

        public long Disk { get; init; }

        public string Id { get; init; }

        public string Level { get; init; }

        public int MaxCpu { get; init; }

        public long MaxDisk { get; init; }

        public long MaxMem { get; init; }

        public long Mem { get; init; }

        public string Node { get; init; }

        [JsonPropertyName("ssl_fingerprint")]
        public string SSLFingerprint { get; init; }

        public NodeStatus Status { get; init; }

        public int UpTime { get; init; }
    }

    public sealed class ObjectResponse<T>
    {
        public T Data { get; init; }
    }

    public sealed class PoolResponse : JsonResponse
    {
        public string Comment { get; init; }

        public string PoolId { get; init; }
    }

    public sealed class StorageResponse : JsonResponse
    {
        public string Content { get; init; }

        public string Digest { get; init; }

        [JsonIgnore]
        public bool Disable => DisableInt == 1;

        [JsonIgnore]
        public bool KRBD => KRBDInt == 1;

        public string Path { get; init; }

        public string Pool { get; init; }

        [JsonIgnore]
        public bool Shared => SharedInt == 1;

        public string Storage { get; init; }

        public StorageType Type { get; init; }

        [JsonPropertyName("disable")]
        private int DisableInt { get; init; }

        [JsonPropertyName("krbd")]
        private int KRBDInt { get; init; }

        [JsonPropertyName("shared")]
        private int SharedInt { get; init; }
    }

    public sealed class TicketResponse : JsonResponse
    {
        public string CSRFPreventionToken { get; init; }

        public string Ticket { get; init; }
    }

    public sealed class VersionResponse : JsonResponse
    {
        public string Release { get; init; }

        public string RepoId { get; init; }

        public string Version { get; init; }
    }

    public sealed class VirtualMachineResponse : JsonResponse
    {
        public double CPUs { get; init; }

        public string Lock { get; init; }

        [JsonPropertyName("running-machine")]
        public string MachineType { get; init; }

        public long MaxDisk { get; init; }

        public long MaxMem { get; init; }

        public string Name { get; init; }

        public int PID { get; init; }

        [JsonPropertyName("running-qemu")]
        public string QEMUVersion { get; init; }

        public string QMPStatus { get; init; }

        public string Tags { get; init; }

        public int Uptime { get; init; }

        public int VMId { get; init; }
    }
}