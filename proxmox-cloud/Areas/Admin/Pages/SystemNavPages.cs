using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using static System.IO.Path;

namespace proxmox_cloud.Areas.Admin.Pages
{
    public class SystemNavPages
    {
        public const string Flavors = "Flavors";
        public const string Hypervisors = "Hypervisors";
        public const string Images = "Images";
        public const string Index = "Index";
        public const string Instances = "Instances";
        public const string SystemInfo = "SystemInfo";
        public const string Volumes = "Volumes";

        public static string FlavorsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Flavors);

        public static string HypervisorsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Hypervisors);

        public static string ImagesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Images);

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string InstancesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Instances);

        public static string SystemInfoNavClass(ViewContext viewContext) => PageNavClass(viewContext, SystemInfo);

        public static string VolumesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Volumes);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}