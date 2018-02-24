using System;
using System.Collections.Generic;
using System.Reflection;
using Xer.Cqrs.Extensions.SimpleInjector;

namespace SimpleInjector
{
    public static class SimpleInjectorContainerExtensions
    {
        public static Container RegisterCqrs(this Container container, Assembly assembly)
        {
            return RegisterCqrs(container, new[] { assembly });
        }

        public static Container RegisterCqrs(this Container container, IEnumerable<Assembly> assemblies)
        {
            RegisterCqrsCore(container, assemblies)
                .RegisterCommandHandlers()
                .RegisterEventHandlers();

            return container;
        }

        public static ICqrsBuilder RegisterCqrsCore(this Container container, Assembly assembly)
        {            
            return RegisterCqrsCore(container, new[] { assembly });
        }

        public static ICqrsBuilder RegisterCqrsCore(this Container container, IEnumerable<Assembly> assemblies)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            return new CqrsBuilder(container, assemblies);
        }
    }
}
