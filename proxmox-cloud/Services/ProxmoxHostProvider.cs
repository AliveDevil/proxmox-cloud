using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Immutable;

namespace proxmox_cloud.Services
{
    public class ProxmoxHostProvider
    {
        private readonly ImmutableArray<Uri> hosts;
        private readonly IHostSelector selector;

        public ProxmoxHostProvider(IConfiguration configuration)
        {
            var proxmox = configuration.GetSection("Proxmox");
            var section = proxmox.GetSection("Cluster");
            var api = proxmox.GetSection("Api");
            var hosts = section.GetSection("Hosts").GetChildren();

            var hostsArray = ImmutableArray.CreateBuilder<Uri>();
            foreach (var host in hosts)
            {
                if (!Uri.TryCreate(host.Value, UriKind.Absolute, out var uri))
                {
                    continue;
                }
                hostsArray.Add(uri);
            }
            this.hosts = hostsArray.ToImmutable();
            selector = Select(api.GetValue("LoadBalance", Mode.Random), this);
        }

        public enum Mode
        {
            Random,
            RoundRobin
        }

        private interface IHostSelector
        {
            Uri Get();
        }

        public Uri Get() => selector.Get();

        private static IHostSelector Select(Mode mode, ProxmoxHostProvider provider) => mode switch
        {
            Mode.Random => new RandomHostSelector(provider),
            Mode.RoundRobin => new RoundRobinSelector(provider),
            _ => throw new NotImplementedException()
        };

        private class RandomHostSelector : IHostSelector
        {
            private readonly int mod;
            private readonly ProxmoxHostProvider provider;
            private readonly Random random = new();

            public RandomHostSelector(ProxmoxHostProvider provider)
            {
                this.provider = provider;
                mod = provider.hosts.Length;
            }

            public Uri Get() => provider.hosts[random.Next(mod)];
        }

        private class RoundRobinSelector : IHostSelector
        {
            private readonly int mod;
            private readonly ProxmoxHostProvider provider;
            private int c;

            public RoundRobinSelector(ProxmoxHostProvider provider)
            {
                this.provider = provider;
                mod = provider.hosts.Length;
                c = mod;
            }

            public Uri Get() => provider.hosts[c = (c + 1) % mod];
        }
    }
}