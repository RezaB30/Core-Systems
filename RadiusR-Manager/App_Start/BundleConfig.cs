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

            bundles.Add(new ScriptBundle("~/bundles/scripts")
                .IncludeDirectory("~/Scripts/Custom", "*.js")
                .IncludeDirectory("~/Scripts/Views", "*.js")
                .Include("~/Scripts/initialize.js"));

            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/Content/css/*.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/views/*.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/bundles/css-m")
                .Include("~/Content/css-m/*.css", new CssRewriteUrlTransform()));

            // view bundles
            bundles.Add(new ScriptBundle("~/bundles/views/customer-register.js").Include("~/Scripts/Views/customer-register.js"));
        }
    }
}
