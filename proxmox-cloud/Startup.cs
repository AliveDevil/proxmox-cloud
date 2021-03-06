using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using proxmox_cloud.CephApi;
using proxmox_cloud.Data;
using proxmox_cloud.ProxmoxApi;
using proxmox_cloud.Services;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().RequireAuthorization();
                endpoints.MapFallback(context =>
                {
                    context.Response.Redirect("/Project");
                    return Task.CompletedTask;
                });
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

            services.AddSingleton<ProxmoxScraper>();
            services.AddSingleton<PveClientFactory>();
            services.AddSingleton<ProxmoxHostProvider>();

            services.AddRefitClient<IPveClient>(new RefitSettings()
            {
                ContentSerializer = new SystemTextJsonContentSerializer()
            }).ConfigureHttpClient(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Proxmox-Cloud");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }).AddHttpMessageHandler(s => ActivatorUtilities.CreateInstance<ProxmoxAuthenticator>(s));

            services.AddHttpClient<CephClient>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Proxmox-Cloud");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }).AddHttpMessageHandler(s => ActivatorUtilities.CreateInstance<CephAuthenticator.Handler>(s))
            .AddPolicyHandler((provider, request) =>
            {
                return Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
                    .RetryAsync(1, (response, retryCount, context) =>
                    {
                        var client = provider.GetRequiredService<CephClient>();
                    });
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", new AuthorizationPolicyBuilder()
                    .RequireRole("Administrator")
                    .Build());
            });
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Admin", "/", "Admin");
            });
        }
    }
}