// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline
{
    using System.Web.Mvc;

    using DotNetNuke.Common;
    using DotNetNuke.DependencyInjection;
    using DotNetNuke.Web.MvcPipeline.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configures services and MVC module controls used by the DNN MVC wrapper library.
    /// </summary>
    /// <remarks>
    /// This class is compatible with DNN 9.x and later. It implements IDnnStartup for automatic
    /// service registration when the assembly is loaded by DNN's dependency injection system.
    /// If your DNN 9.x version doesn't support IDnnStartup, you may need to manually call
    /// ConfigureServices from your own startup code.
    /// </remarks>
    public class Startup : IDnnStartup
    {
        /// <summary>
        /// Configures services for the MVC wrapper library.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Register all IMvcModuleControl implementations found in loaded assemblies
            services.AddMvcModuleControls();

            // Set up MVC dependency resolver to use DNN's dependency provider
            // This allows MVC to resolve controllers and other components from the DI container
            //if (Globals.DependencyProvider != null)
            //{
            //    DependencyResolver.SetResolver(new DnnMvcWrapperDependencyResolver(Globals.DependencyProvider));
            //}
        }
    }
}
