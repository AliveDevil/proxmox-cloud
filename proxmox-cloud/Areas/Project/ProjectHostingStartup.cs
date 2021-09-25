using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(proxmox_cloud.Areas.Project.ProjectHostingStartup))]

namespace proxmox_cloud.Areas.Project
{
    public class ProjectHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}