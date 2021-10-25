using Microsoft.Extensions.Hosting;
using proxmox_cloud.ProxmoxApi;
using proxmox_cloud.System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class ProxmoxScraper : IHostedService
    {
        private readonly ReaderWriterLockSlim atomic = new();
        private readonly PveClientFactory clientFactory;
        private readonly Timer timer;
        private Scrape scrape;

        public ProxmoxScraper(PveClientFactory clientFactory)
        {
            timer = new(Handler);
            this.clientFactory = clientFactory;
        }

        public delegate void CollectAction(in CollectContext context);

        public void Collect(CollectAction action)
        {
            CollectContext context = new(this);
            using (atomic.UseReadLock())
            {
                action(context);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void Handler(object state)
        {
            var resources = clientFactory.Get().GetClusterResources().Result.Data;
            var types = resources.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => (IEnumerable<ClusterResource>)x);
            IEnumerable<HypervisorModel> hypervisors = Enumerable.Empty<HypervisorModel>();
            if (types.TryGetValue(ClusterResourceType.Node, out var hypervisorResources))
            {
                hypervisors = hypervisorResources.Select(_ => new HypervisorModel(_)).ToList();
            }
            IEnumerable<ZoneModel> zones = Enumerable.Empty<ZoneModel>();
            if (types.TryGetValue(ClusterResourceType.SDN, out var sdnResources))
            {
                zones = sdnResources.Select(_ => new ZoneModel(_)).ToList();
            }

            using (atomic.UseWriteLock())
            {
                scrape = new()
                {
                    Hypervisors = hypervisors,
                    Zones = zones
                };
            }
        }

        public ref struct CollectContext
        {
            private readonly ProxmoxScraper scraper;

            public CollectContext(ProxmoxScraper scraper)
            {
                this.scraper = scraper;
            }

            public IEnumerable<HypervisorModel> Hypervisors => Get(_ => _.Hypervisors);

            public IEnumerable<ZoneModel> Zones => Get(_ => _.Zones);

            private T Get<T>(Expression<Func<Scrape, T>> expr) => expr.Body is MemberExpression member &&
                member.Member is PropertyInfo property ? (T)property.GetValue(scraper.scrape) : default;
        }

        public struct Scrape
        {
            public IEnumerable<HypervisorModel> Hypervisors { get; init; }

            public IEnumerable<ZoneModel> Zones { get; init; }
        }
    }
}