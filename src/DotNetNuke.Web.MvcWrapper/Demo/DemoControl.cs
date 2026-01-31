using DotNetNuke.Web.MvcPipeline.ModuleControl.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNuke.Web.MvcPipeline.ModuleControl
{
    public class DemoControl : RazorModuleControlBase
    {
        /// <inheritdoc />
        public override IRazorModuleResult Invoke()
        {
            return this.Content("DemoView");
            // return View(new DemoModel());
        }
    }
}
