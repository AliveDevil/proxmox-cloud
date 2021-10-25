namespace proxmox_cloud.ProxmoxApi
{
    public class ZoneModel
    {
        public ZoneModel(ClusterResource resource)
        {
            Zone = (string)resource.AdditionalProperties["sdn"];
        }

        public ZoneModel(ZoneResponse zone)
        {
            DNS = zone.DNS;
            DNSZone = zone.DNSZone;
            IPAM = zone.IPAM;
            MTU = zone.MTU;
            Nodes = zone.Nodes;
            Pending = zone.Pending;
            ReverseDNS = zone.ReverseDNS;
            State = zone.State;
            Type = zone.Type;
            Zone = zone.Zone;
        }

        public string DNS { get; }

        public string DNSZone { get; }

        public string IPAM { get; }

        public int MTU { get; }

        public string Nodes { get; }

        public bool Pending { get; }

        public string ReverseDNS { get; }

        public string State { get; }

        public SDNZoneType Type { get; }

        public string Zone { get; }
    }
}