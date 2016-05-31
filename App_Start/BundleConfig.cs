using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace E_2
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                                  "~/Content/cube.css",
                                  "~/Content/page.css"));

            bundles.Add(new ScriptBundle("~/bundles/cubeHead").Include(
                      "~/scripts/cubes/tween.js",
                      "~/scripts/cubes/Three.noStrict.js",
                      "~/scripts/cubes/cuber.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/cube").Include(
                      "~/scripts/cubes/iecss3d.js",
                      "~/scripts/cubes/ierenderer.js",
                      "~/scripts/cubes/deviceMotion.js",
                      "~/scripts/cubes/locked.js",
                      "~/scripts/cubes/frames.js",
                      "~/scripts/cubes/main.js"
                      ));

            BundleTable.EnableOptimizations = true;
        }
    }
}