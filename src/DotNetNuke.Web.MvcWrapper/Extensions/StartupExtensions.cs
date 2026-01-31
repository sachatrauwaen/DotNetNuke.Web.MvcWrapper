// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.Extensions
{
    using System.Linq;

    using DotNetNuke.DependencyInjection.Extensions;
    using DotNetNuke.Instrumentation;
    using DotNetNuke.Web.MvcPipeline.ModuleControl;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Adds DNN MVC module control specific startup extensions to simplify service registration.
    /// </summary>
    public static class StartupExtensions
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(StartupExtensions));

        /// <summary>
        /// Configures all <see cref="IMvcModuleControl"/> implementations to be used with the Service Collection for Dependency Injection.
        /// </summary>
        /// <param name="services">Service Collection used to register services in the container.</param>
        /// <remarks>
        /// This method scans all loaded assemblies for types that implement <see cref="IMvcModuleControl"/>
        /// and registers them as transient services in the DI container.
        /// </remarks>
        public static void AddMvcModuleControls(this IServiceCollection services)
        {
            var allTypes = TypeExtensions.SafeGetTypes();
            allTypes.LogOtherExceptions(Logger);

            var mvcControlTypes = allTypes.Types
                .Where(
                    type => typeof(IMvcModuleControl).IsAssignableFrom(type) &&
                            type is { IsClass: true, IsAbstract: false });
            
            foreach (var controller in mvcControlTypes)
            {
                services.TryAddTransient(controller);
                Logger.Trace($"Registered MVC Module Control: {controller.FullName}");
            }
        }
    }
}
