using Satrabel.Web.MvcPipeline.ModuleControl;
using Satrabel.Web.MvcPipeline.ModuleControl.Razor;

namespace SampleViewModule
{
    public class ViewControl : RazorModuleControlBase
    {
        /// <inheritdoc />
        public override IRazorModuleResult Invoke()
        {
            return this.View();
        }
    }
}
