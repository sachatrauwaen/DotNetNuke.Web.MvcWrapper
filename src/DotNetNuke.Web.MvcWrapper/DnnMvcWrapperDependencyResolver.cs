// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Custom dependency resolver for ASP.NET MVC that integrates with DNN's dependency injection container.
    /// </summary>
    /// <remarks>
    /// This resolver allows ASP.NET MVC to resolve controllers and other components from the DNN DI container.
    /// It falls back to the default MVC dependency resolver if a service cannot be resolved from the DNN container.
    /// </remarks>
    internal class DnnMvcWrapperDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDependencyResolver fallbackResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnnMvcWrapperDependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceProvider">The DNN service provider.</param>
        public DnnMvcWrapperDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.fallbackResolver = DependencyResolver.Current;
        }

        /// <inheritdoc/>
        public object GetService(Type serviceType)
        {
            // Try to resolve from DNN's DI container first
            var service = this.serviceProvider.GetService(serviceType);
            
            // Fall back to the default MVC resolver if not found
            return service ?? this.fallbackResolver?.GetService(serviceType);
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            // Get services from DNN's DI container
            var services = this.serviceProvider.GetServices(serviceType)?.ToList() ?? new List<object>();
            
            // Merge with services from the default MVC resolver
            var fallbackServices = this.fallbackResolver?.GetServices(serviceType);
            if (fallbackServices != null)
            {
                services.AddRange(fallbackServices);
            }

            return services;
        }
    }
}
