using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using static System.IO.Path;

namespace proxmox_cloud.Areas.Admin.Pages.Identity
{
    public class IdentityNavPages
    {
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
