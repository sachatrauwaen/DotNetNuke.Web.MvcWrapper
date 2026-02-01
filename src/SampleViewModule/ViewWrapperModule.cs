using DotNetNuke.Entities.Modules;
using Satrabel.Web.MvcPipeline.ModuleControl;
using Satrabel.Web.MvcPipeline.ModuleControl.WebForms;
using Microsoft.Extensions.DependencyInjection;

namespace SampleViewModule
{
    public class ViewWrapperModule : WrapperModule
    {
        protected override IMvcModuleControl CreateModuleControl(ModuleInfo moduleConfiguration)
        {
            return this.DependencyProvider.GetService<ViewControl>();
        }
    }
}
