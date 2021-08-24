using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using proxmox_cloud.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace proxmox_cloud.ProxmoxApi
{
    public class ApiException : Exception
    {
        public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, System.Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public string Response { get; private set; }

        public int StatusCode { get; private set; }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    public class ApiException<TResult> : ApiException
    {
        public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }

        public TResult Result { get; private set; }
    }

    public class PveClient
    {
        private readonly HttpClient _httpClient;
        private readonly ProxmoxHostProvider hostProvider;

        public PveClient(HttpClient httpClient, ProxmoxHostProvider hostProvider)
        {
            _httpClient = httpClient;
            this.hostProvider = hostProvider;
        }

        public enum NodeStatus { Unknown, Offline, Online }

        public string BaseUrl { get; set; } = "/api2/json/";

        public Task<TicketResponse> CreateAccessTicketAsync(string username, string password) => CreateAccessTicketAsync(username, password, CancellationToken.None);

        public async Task<TicketResponse> CreateAccessTicketAsync(string username, string password, CancellationToken cancellationToken)
        {
            StringBuilder urlBuilder_ = new();
            urlBuilder_.Append(BaseUrl?.TrimEnd('/')).Append("/access/ticket");

            var client_ = _httpClient;
            using HttpRequestMessage request_ = new()
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    ["username"] = username,
                    ["password"] = password
                }),
                Method = HttpMethod.Post,
                Headers =
                {
                    Accept =
                    {
                        new("application/json")
                    }
                },
            };

            var url_ = urlBuilder_.ToString();
            request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

            PrepareRequest(client_, request_, url_);

            using var response_ = await client_.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            var headers_ = Enumerable.ToDictionary(response_.Headers, h => h.Key, h => h.Value);
            if (response_.Content != null && response_.Content.Headers != null)
            {
                foreach (var item in response_.Content.Headers)
                    headers_[item.Key] = item.Value;
            }
            response_.EnsureSuccessStatusCode();

            var responseText = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var doc = JObject.Parse(responseText);
                return doc.GetValue("data").ToObject<TicketResponse>();
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + nameof(TicketResponse) + ".";
                throw new ApiException(message, (int)response_.StatusCode, responseText, headers_, exception);
            }
        }

        public Task<int> GetClusterNextIdAsync() => GetClusterNextIdAsync(CancellationToken.None);

        public async Task<int> GetClusterNextIdAsync(CancellationToken cancellationToken)
        {
            StringBuilder urlBuilder_ = new();
            urlBuilder_.Append(BaseUrl?.TrimEnd('/')).Append("/cluster/nextid");

            var client_ = _httpClient;
            using HttpRequestMessage request_ = new()
            {
                Method = HttpMethod.Get,
                Headers =
                {
                    Accept =
                    {
                        new("application/json")
                    }
                },
            };

            var url_ = urlBuilder_.ToString();
            request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

            PrepareRequest(client_, request_, url_);

            using var response_ = await client_.SendAsync(request_, cancellationToken);
            var headers_ = Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
            if (response_.Content != null && response_.Content.Headers != null)
            {
                foreach (var item_ in response_.Content.Headers)
                    headers_[item_.Key] = item_.Value;
            }
            response_.EnsureSuccessStatusCode();

            var responseText = await response_.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var doc = JObject.Parse(responseText);
                return (int)doc.GetValue("data");
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + nameof(Int32) + ".";
                throw new ApiException(message, (int)response_.StatusCode, responseText, headers_, exception);
            }
        }

        public Task<ICollection<NodeResponse>> GetNodesAsync() => GetNodesAsync(CancellationToken.None);

        public async Task<ICollection<NodeResponse>> GetNodesAsync(CancellationToken cancellationToken)
        {
            StringBuilder urlBuilder_ = new();
            urlBuilder_.Append(BaseUrl?.TrimEnd('/')).Append("/nodes");

            var client_ = _httpClient;
            using HttpRequestMessage request_ = new()
            {
                Method = HttpMethod.Get,
                Headers =
                {
                    Accept =
                    {
                        new("application/json")
                    }
                }
            };

            var url_ = urlBuilder_.ToString();
            request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

            PrepareRequest(client_, request_, url_);

            using var response_ = await client_.SendAsync(request_, cancellationToken).ConfigureAwait(false);
            var headers_ = Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
            if (response_.Content != null && response_.Content.Headers != null)
            {
                foreach (var item_ in response_.Content.Headers)
                    headers_[item_.Key] = item_.Value;
            }
            response_.EnsureSuccessStatusCode();

            var responseText = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var doc = JObject.Parse(responseText);
                return doc.GetValue("data").ToObject<Collection<NodeResponse>>();
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + nameof(Collection<NodeResponse>) + ".";
                throw new ApiException(message, (int)response_.StatusCode, responseText, headers_, exception);
            }
        }

        public Task<VersionResponse> GetVersionAsync() => GetVersionAsync(CancellationToken.None);

        public async Task<VersionResponse> GetVersionAsync(CancellationToken cancellationToken)
        {
            StringBuilder urlBuilder_ = new();
            urlBuilder_.Append(BaseUrl?.TrimEnd('/')).Append("/version");

            var client_ = _httpClient;
            HttpRequestMessage request_ = new()
            {
                Method = HttpMethod.Get,
                Headers =
                {
                    Accept =
                    {
                        new("application/json")
                    }
                }
            };

            var url_ = urlBuilder_.ToString();
            request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

            PrepareRequest(client_, request_, url_);

            using var response_ = await client_.SendAsync(request_, cancellationToken).ConfigureAwait(false);
            var headers_ = Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
            if (response_.Content != null && response_.Content.Headers != null)
            {
                foreach (var item_ in response_.Content.Headers)
                    headers_[item_.Key] = item_.Value;
            }
            response_.EnsureSuccessStatusCode();

            var responseText = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var doc = JObject.Parse(responseText);
                return doc.GetValue("data").ToObject<VersionResponse>();
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + nameof(VersionResponse) + ".";
                throw new ApiException(message, (int)response_.StatusCode, responseText, headers_, exception);
            }
        }

        private void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            if (!request.RequestUri.IsAbsoluteUri)
            {
                request.RequestUri = new(hostProvider.Get(), url);
            }
        }

        public abstract class JsonResponse
        {
            [JsonExtensionData]
            public Dictionary<string, object> AdditionalProperties { get; } = new();
        }

        public sealed class NodeResponse : JsonResponse
        {
            public double Cpu { get; init; }

            public long Disk { get; init; }

            public string Id { get; init; }

            public string Level { get; init; }

            public int MaxCpu { get; init; }

            public long MaxDisk { get; init; }

            public long MaxMem { get; init; }

            public long Mem { get; init; }

            public string Node { get; init; }

            [JsonProperty("ssl_fingerprint")]
            public string SSLFingerprint { get; init; }

            public NodeStatus Status { get; init; }

            public int UpTime { get; init; }
        }

        public class TicketResponse : JsonResponse
        {
            public string CSRFPreventionToken { get; init; }

            public string Ticket { get; init; }
        }

        public sealed class VersionResponse : JsonResponse
        {
            public string Release { get; init; }

            public string RepoId { get; init; }

            public string Version { get; init; }
        }
    }
}