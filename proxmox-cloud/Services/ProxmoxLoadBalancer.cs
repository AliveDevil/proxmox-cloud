using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class ProxmoxLoadBalancer : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}