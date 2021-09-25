using Microsoft.Extensions.Hosting;
using proxmox_cloud.ProxmoxApi;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class ProxmoxLoadBalancer : IHostedService
    {
        private readonly PveClientFactory clientFactory;

        public ProxmoxLoadBalancer(PveClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}