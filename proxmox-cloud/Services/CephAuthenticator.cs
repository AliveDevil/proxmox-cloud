using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class CephAuthenticator
    {
        public async Task AuthenticateAsync(HttpRequestMessage request)
        {
        }

        public class Handler : DelegatingHandler
        {
            private readonly CephAuthenticator authenticator;

            public Handler(CephAuthenticator authenticator)
            {
                this.authenticator = authenticator;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                await authenticator.AuthenticateAsync(request);

                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}