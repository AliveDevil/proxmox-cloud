using System.Net.Http;

namespace proxmox_cloud.CephApi
{
    public class CephClient
    {
        private readonly HttpClient httpClient;

        public CephClient(HttpClient _httpClient)
        {
            httpClient = _httpClient;
        }
    }
}
