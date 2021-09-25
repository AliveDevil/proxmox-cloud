using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proxmox_cloud.ProxmoxApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxmox_cloud.Areas.Admin.Pages.System
{
    public class HypervisorsModel : PageModel
    {
        private readonly PveClientFactory client;

        public HypervisorsModel(PveClientFactory client)
        {
            this.client = client;
        }

        public double CPU { get; set; }

        public List<HypervisorModel> Hypervisors { get; set; }

        public double MaxCPU { get; set; }

        public long MaxMemory { get; set; }

        public long Memory { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var resources = await client.Get().GetClusterResources();

            var nodes = resources.Data.Where(r => r.Type == ClusterResourceType.Node);
            MaxMemory = nodes.Sum(x => x.MaxMem);
            Memory = nodes.Sum(x => x.Mem);
            MaxCPU = nodes.Sum(x => x.MaxCPU);
            CPU = nodes.Sum(x => x.CPU);

            Hypervisors = nodes.Select(r => new HypervisorModel(
                r.Node, r.MaxCPU, r.CPU, r.MaxMem, r.Mem,
                resources.Data.Where(r => r.Type == ClusterResourceType.Qemu).Count(x => x.Node == r.Node))
            ).ToList();

            return Page();
        }

        public record HypervisorModel(string Hostname, double MaxCPU, double CPU, long MaxMemory, long Memory, int Instances);
    }
}