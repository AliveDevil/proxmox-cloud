using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(proxmox_cloud.Areas.Admin.AdminHostingStartup))]

namespace proxmox_cloud.Areas.Admin
{
    public class AdminHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}