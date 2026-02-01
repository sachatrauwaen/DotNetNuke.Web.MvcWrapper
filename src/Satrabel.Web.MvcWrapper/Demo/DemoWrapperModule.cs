using DotNetNuke.Entities.Modules;
using Satrabel.Web.MvcPipeline.ModuleControl;
using Satrabel.Web.MvcPipeline.ModuleControl.WebForms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satrabel.Web.MvcPipeline.Demo
{
    /// <summary>
    /// Demo wrapper module that hosts the MVC Razor demo control.
    /// </summary>
    public class DemoWrapperModule : WrapperModule
    {
        /// <summary>
        /// Creates the MVC module control (DemoControl) for the given module configuration.
        /// </summary>
        /// <param name="moduleConfiguration">The module configuration.</param>
        /// <returns>The MVC module control instance.</returns>
        protected override IMvcModuleControl CreateModuleControl(ModuleInfo moduleConfiguration)
        {
            return this.DependencyProvider.GetService<DemoControl>();
        }
    }
}
