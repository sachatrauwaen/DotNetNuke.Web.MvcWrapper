using Satrabel.Web.MvcPipeline.ModuleControl.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satrabel.Web.MvcPipeline.ModuleControl
{
    /// <summary>
    /// Demo MVC module control for testing the Razor wrapper.
    /// </summary>
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
