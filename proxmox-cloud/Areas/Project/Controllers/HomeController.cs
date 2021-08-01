using Microsoft.AspNetCore.Mvc;

namespace proxmox_cloud.Areas.Project.Controllers
{
    [Area("Project")]
    public class HomeController : Controller
    {
        public IActionResult Fallback() => RedirectToAction("Index");

        public IActionResult Index()
        {
            return View();
        }
    }
}