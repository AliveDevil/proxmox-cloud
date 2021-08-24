using Microsoft.Extensions.Configuration;
using proxmox_cloud.ProxmoxApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.Services
{
    public class ProxmoxAuthenticator
    {
        private readonly PveClient apiClient;
        private readonly ManualResetEventSlim eager = new(true);
        private readonly AutoResetEvent gate = new(true);
        private readonly HttpClient httpClient;
        private readonly ReaderWriterLockSlim reader = new();
        private readonly NetworkCredential serviceCredential;
        private DateTimeOffset lastAuthentication;
        private PveClient.TicketResponse token;

        public ProxmoxAuthenticator(HttpClient httpClient, ProxmoxHostProvider hostProvider, IConfiguration configuration)
        {
            apiClient = new(httpClient, hostProvider);
            this.httpClient = httpClient;

            var serviceSection = configuration.GetSection("Proxmox").GetSection("Service");
            serviceCredential = new(
                serviceSection.GetValue<string>("Username"),
                serviceSection.GetValue<string>("Password"));
        }

        public string BaseUrl { get; set; } = "/api2/json/";

        public async Task AuthenticateAsync(HttpRequestMessage request)
        {
            await eager.WaitHandle.WaitOneAsync().ConfigureAwait(false);
            PveClient.TicketResponse auth;
            reader.EnterReadLock();
            auth = token;
            reader.ExitReadLock();

            if (gate.WaitOne(0))
            {
                if ((DateTimeOffset.Now - lastAuthentication).TotalMinutes > 110)
                {
                    eager.Reset();

                    auth = await apiClient.CreateAccessTicketAsync(serviceCredential.UserName, serviceCredential.Password);

                    reader.EnterWriteLock();
                    lastAuthentication = DateTimeOffset.Now;
                    token = auth;
                    reader.ExitWriteLock();

                    eager.Set();
                }
                gate.Set();
            }

            request.Headers.Add("cookie", "PVEAuthCookie=" + auth.Ticket);
            request.Headers.Add("CSRFPreventionToken", auth.CSRFPreventionToken);
        }

        public class Handler : DelegatingHandler
        {
            private readonly ProxmoxAuthenticator authenticator;

            public Handler(ProxmoxAuthenticator authenticator)
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