using Microsoft.AspNetCore.Mvc.RazorPages;
using proxmox_cloud.ProxmoxApi;
using System.Threading.Tasks;

namespace proxmox_cloud.Areas.Admin.Pages.System
{
    public class NetworksModel : PageModel
    {
        private readonly PveClientFactory pveClient;

        public NetworksModel(PveClientFactory pveClient)
        {
            this.pveClient = pveClient;
        }

        public void OnGet()
        {
        }
    }
}