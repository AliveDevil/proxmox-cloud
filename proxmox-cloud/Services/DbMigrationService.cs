using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using proxmox_cloud.Data;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class DbMigrationService : IHostedService
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public DbMigrationService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var context = contextFactory.CreateDbContext();
            await context.Database.MigrateAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}