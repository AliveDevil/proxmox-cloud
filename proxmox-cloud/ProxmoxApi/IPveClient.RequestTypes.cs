using Newtonsoft.Json;
using System.Collections.Generic;

namespace proxmox_cloud.ProxmoxApi
{
    public abstract class JsonRequest
    {
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; } = new();
    }

    public sealed class NodeConfigRequest : JsonRequest
    {
    }
}