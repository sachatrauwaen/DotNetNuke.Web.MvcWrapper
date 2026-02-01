// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.Extensions
{
    using DotNetNuke.DependencyInjection.Extensions;
    using DotNetNuke.Framework.Reflections;
    using DotNetNuke.Instrumentation;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Satrabel.Web.MvcPipeline.ModuleControl;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

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
#if DNN10
            var allTypes = TypeExtensions.SafeGetTypes();

            allTypes.LogOtherExceptions(Logger);

            var mvcControlTypes = allTypes.Types
                .Where(
                    type => typeof(IMvcModuleControl).IsAssignableFrom(type) &&
                            type is { IsClass: true, IsAbstract: false });
#else
            var typeLocator = new TypeLocator();
            IEnumerable<Type> mvcControlTypes = typeLocator.GetAllMatchingTypes(IsValidModuleControl);

#endif
            foreach (var controller in mvcControlTypes)
            {
                services.TryAddTransient(controller);
                Logger.Trace($"Registered MVC Module Control: {controller.FullName}");
            }
        }

        private static bool IsValidModuleControl(Type t)
        {
            return t != null &&
                    t.IsClass && 
                    !t.IsAbstract &&
                    t.IsVisible && typeof(IMvcModuleControl).IsAssignableFrom(t);
        }
    }
}
