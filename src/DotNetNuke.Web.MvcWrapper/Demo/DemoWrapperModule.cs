using DotNetNuke.Entities.Modules;
using DotNetNuke.Web.MvcPipeline.ModuleControl;
using DotNetNuke.Web.MvcPipeline.ModuleControl.WebForms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNuke.Web.MvcPipeline.Demo
{
    public class DemoWrapperModule : WrapperModule
    {
        protected override IMvcModuleControl CreateModuleControl(ModuleInfo moduleConfiguration)
        {
            return this.DependencyProvider.GetService<DemoControl>();
        }
    }
}
