using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class ProxmoxAuthenticator : DelegatingHandler
    {
        private readonly NetworkCredential serviceCredential;

        public ProxmoxAuthenticator(IConfiguration configuration)
        {
            var serviceSection = configuration.GetSection("Proxmox").GetSection("Service");
            serviceCredential = new(
                serviceSection.GetValue<string>("ApiToken"),
                serviceSection.GetValue<string>("ApiSecret"));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new("PVEAPIToken",
                serviceCredential.UserName + "=" + serviceCredential.Password);

            return base.SendAsync(request, cancellationToken);
        }
    }
}