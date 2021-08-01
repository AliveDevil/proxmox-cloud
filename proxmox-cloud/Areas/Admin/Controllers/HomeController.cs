using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace proxmox_cloud.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}