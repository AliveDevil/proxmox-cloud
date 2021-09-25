using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace proxmox_cloud.ProxmoxApi
{
    public partial interface IPveClient : IDisposable
    {
        HttpClient Client { get; }

        [Get("/cluster/nextid")]
        Task<ObjectResponse<int>> GetClusterNextId();

        [Get("/cluster/options")]
        Task<ObjectResponse<ClusterOptions>> GetClusterOptions();

        [Get("/cluster/resources")]
        Task<ArrayResponse<ClusterResource>> GetClusterResources();

        [Get("/cluster/status")]
        Task<ArrayResponse<ClusterStatusResponse>> GetClusterStatus();

        [Get("/nodes/{node}")]
        Task<ObjectResponse<NodeResponse>> GetNode(string node);

        [Get("/nodes/{node}/config")]
        Task<ObjectResponse<NodeConfigResponse>> GetNodeConfig(string node);

        [Get("/nodes")]
        Task<ArrayResponse<NodeResponse>> GetNodes();

        [Get("/nodes/{node}/version")]
        Task<ObjectResponse<VersionResponse>> GetNodeVersion(string node);

        [Get("/nodes/{node}/qemu")]
        Task<ArrayResponse<VirtualMachineResponse>> GetNodeVirtualMachines(string node);

        [Get("/pools/{pool}")]
        Task<ObjectResponse<PoolResponse>> GetPool(string pool);

        [Get("/pools")]
        Task<ArrayResponse<PoolResponse>> GetPools();

        [Get("/storage/{storage}")]
        Task<ObjectResponse<StorageResponse>> GetStorage(string storage);

        [Get("/storage")]
        Task<ArrayResponse<StorageResponse>> GetStorages();

        [Get("/version")]
        Task<ObjectResponse<VersionResponse>> GetVersion();
    }
}