using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class AdminUserService : IHostedService
    {
        private readonly ILogger<AdminUserService> logger;
        private readonly IServiceScopeFactory scopeFactory;

        public AdminUserService(ILogger<AdminUserService> logger, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<IdentityUser>>();

            if (await roleManager.FindByNameAsync("Administrator") is not IdentityRole)
            {
                IdentityRole role = new("Administrator");
                if (await roleManager.CreateAsync(role) is IdentityResult result && result != IdentityResult.Success)
                {
                    // TODO: Error
                }
            }
            var users = await userManager.GetUsersInRoleAsync("Administrator");
            if (users.Count == 0)
            {
                IdentityUser user = new("admin");
                user.PasswordHash = hasher.HashPassword(user, "Proxmox");
                if (await userManager.CreateAsync(user) is IdentityResult result && result != IdentityResult.Success)
                {
                    // TODO: Error
                }
                logger.Log(LogLevel.Information, "PLEASE CHANGE! `admin` `password is `Proxmox`.");
                await userManager.AddToRoleAsync(user, "Administrator");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}