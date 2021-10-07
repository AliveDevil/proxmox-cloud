using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proxmox_cloud.Services;
using System.Collections.Generic;
using System.Linq;

namespace proxmox_cloud.Areas.Admin.Pages.System
{
    public class HypervisorsModel : PageModel
    {
        private readonly ProxmoxScraper client;

        public HypervisorsModel(ProxmoxScraper client)
        {
            this.client = client;
        }

        public double CPU { get; set; }

        public List<HypervisorModel> Hypervisors { get; set; }

        public double MaxCPU { get; set; }

        public long MaxMemory { get; set; }

        public long Memory { get; set; }

        public IActionResult OnGetAsync()
        {
            client.Collect((in ProxmoxScraper.CollectContext c) =>
            {
                Hypervisors = c.Hypervisors.Select(r => new HypervisorModel(
                    r.Node, r.MaxCpu, r.Cpu, r.MaxMem, r.Mem, 0)).ToList();
            });

            return Page();
        }

        public record HypervisorModel(string Hostname, double MaxCPU, double CPU, long MaxMemory, long Memory, int Instances);
    }
}