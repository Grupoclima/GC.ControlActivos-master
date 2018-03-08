using System.Web.Optimization;

namespace AdministracionActivosSobrantes.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            ////VENDOR RESOURCES

            ////~/Bundles/vendor/css
            //bundles.Add(
            //    new StyleBundle("~/Bundles/vendor/css")
            //        .Include("~/Content/themes/base/all.css", new CssRewriteUrlTransform())
            //        .Include("~/Content/bootstrap.min.css", new CssRewriteUrlTransform())
            //        .Include("~/Content/toastr.css")
            //        .Include("~/Scripts/sweetalert/sweet-alert.css")
            //        .Include("~/Content/flags/famfamfam-flags.css", new CssRewriteUrlTransform())
            //        .Include("~/Content/font-awesome.min.css", new CssRewriteUrlTransform())
            //    );

            ////~/Bundles/vendor/js/top (These scripts should be included in the head of the page)
            //bundles.Add(
            //    new ScriptBundle("~/Bundles/vendor/js/top")
            //        .Include(
            //            "~/Abp/Framework/scripts/utils/ie10fix.js",
            //            "~/Scripts/modernizr-2.8.3.js"
            //        )
            //    );

            //~/Bundles/vendor/bottom (Included in the bottom for fast page load)
            bundles.Add(
                new StyleBundle("~/Bundles/vendor/css")
                //.Include("~/Content/themes/base/all.css", new CssRewriteUrlTransform())
                //.Include("~/Content/bootstrap.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/toastr.css")
                    .Include("~/Scripts/sweetalert/sweet-alert.css")
                //.Include("~/Content/flags/famfamfam-flags.css", new CssRewriteUrlTransform())
                //.Include("~/Content/font-awesome.min.css", new CssRewriteUrlTransform())
                );


            //~/Bundles/vendor/js/top (These scripts should be included in the head of the page)
            //bundles.Add(
            //    new ScriptBundle("~/Bundles/vendor/js/top")
            //        .Include(
            //            "~/Abp/Framework/scripts/utils/ie10fix.js",
            //            "~/Scripts/modernizr-2.8.3.js"
            //        )
            //    );

            //~/Bundles/vendor/bottom (Included in the bottom for fast page load)
            bundles.Add(
                new ScriptBundle("~/Bundles/vendor/js/bottom")
                    .Include(
                        "~/Scripts/json2.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/moment-with-locales.min.js",
                //"~/Scripts/jquery.validate.min.js",
                //"~/Scripts/jquery.blockUI.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/sweetalert/sweet-alert.min.js",
                        "~/Scripts/others/spinjs/spin.js",
                        "~/Scripts/others/spinjs/jquery.spin.js",
                        "~/Scripts/plugins/debounce/debounce.js",
                        "~/Abp/Framework/scripts/abp.js",
                        "~/Abp/Framework/scripts/libs/abp.jquery.js",
                        "~/Abp/Framework/scripts/libs/abp.toastr.js",
                        "~/Abp/Framework/scripts/libs/abp.blockUI.js",
                        "~/Abp/Framework/scripts/libs/abp.sweet-alert.js",
                        "~/Abp/Framework/scripts/libs/abp.spin.js"
                    )
                );

            ////APPLICATION RESOURCES

            ////~/Bundles/css
            //bundles.Add(
            //    new StyleBundle("~/Bundles/css")
            //        .Include("~/css/main.css")
            //    );

            ////~/Bundles/js
            //bundles.Add(
            //    new ScriptBundle("~/Bundles/js")
            //        .Include("~/js/main.js")
            //    );

            // Vendor scripts
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-2.1.1.min.js"));

            //// jQuery Validation
            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //"~/Scripts/jquery.validate.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
                      "~/Scripts/app/inspinia.js"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
                      "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));

            // jQuery plugins
            bundles.Add(new ScriptBundle("~/plugins/metsiMenu").Include(
                      "~/Scripts/plugins/metisMenu/metisMenu.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/pace").Include(
                      "~/Scripts/plugins/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/datatable").Include(
                    "~/Scripts/plugins/dataTables/datatables.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/jSignature").Include(
                    "~/Scripts/plugins/jSignature/jSignature.min.js"));

            //bundles.Add(new ScriptBundle("~/plugins/jSignature").Include(
            //        "~/Scripts/plugins/signature_pad/signature_pad.js",
            //        "~/Scripts/plugins/signature_pad/bezier.js",
            //        "~/Scripts/plugins/signature_pad/point.js",
            //        "~/Scripts/plugins/signature_pad/throttle.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/animate.css",
                      "~/Content/plugins/dataTables/datatables.min.css",
                      "~/Content/plugins/dropzone/basic.css",
                      "~/Content/style.css"));

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                      "~/fonts/font-awesome/css/font-awesome.min.css"));
        }
    }
}