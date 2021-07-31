using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class AdminUserService : IHostedService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public AdminUserService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await roleManager.FindByNameAsync("builtin_admin") is not IdentityRole)
            {
                IdentityRole role = new("builtin_admin");
                if (await roleManager.CreateAsync(role) != IdentityResult.Success)
                {
                    // TODO: Error
                }
            }
            var users = await userManager.GetUsersInRoleAsync("builtin_admin");
            if (users.Count == 0)
            {
                IdentityUser user = new("admin") { EmailConfirmed = true };
                if (await userManager.CreateAsync(user) != IdentityResult.Success)
                {
                    // TODO: Error
                }
                await userManager.AddToRoleAsync(user, "builtin_admin");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}