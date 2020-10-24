using System.Web;
using System.Web.Optimization;

namespace RadiusR_Manager
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").IncludeDirectory(
                        "~/Scripts/jquery", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").IncludeDirectory(
                "~/Scripts/Custom", "*.js").Include(
                "~/Scripts/initialize.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").IncludeDirectory(
                "~/Content/css", "*.css", true));

            bundles.Add(new StyleBundle("~/Content/css-m").IncludeDirectory(
                "~/Content/css-m", "*.css", true
                ));

            // view bundles
            bundles.Add(new ScriptBundle("~/bundles/views/customer-register.js").Include("~/Scripts/Views/customer-register.js"));
        }
    }
}
