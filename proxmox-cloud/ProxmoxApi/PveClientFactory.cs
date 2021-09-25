using Microsoft.Extensions.DependencyInjection;
using proxmox_cloud.Services;
using System;

namespace proxmox_cloud.ProxmoxApi
{
    public sealed class PveClientFactory
    {
        private readonly ProxmoxHostProvider hostProvider;
        private readonly IServiceProvider provider;

        public PveClientFactory(ProxmoxHostProvider hostProvider, IServiceProvider provider)
        {
            this.hostProvider = hostProvider;
            this.provider = provider;
        }

        public IPveClient Get()
        {
            var client = provider.GetService<IPveClient>();
            client.Client.BaseAddress = new Uri(hostProvider.Get(), "/api2/json");
            return client;
        }

        public IPveClient Get(string hostname)
        {
            var client = provider.GetService<IPveClient>();
            UriBuilder urlBuilder = new("https", hostname, 8006, "/api2/json");
            client.Client.BaseAddress = urlBuilder.Uri;
            return client;
        }
    }
}