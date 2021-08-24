using Microsoft.Extensions.Hosting;
using proxmox_cloud.ProxmoxApi;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class ProxmoxLoadBalancer : IHostedService
    {
        private readonly PveClient client;

        public ProxmoxLoadBalancer(PveClient client)
        {
            this.client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var version = await client.GetVersionAsync();
            var nodes = await client.GetNodesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}