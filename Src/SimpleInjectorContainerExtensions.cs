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
            return AddCqrs(container, new[] { assembly });
        }

        public static Container AddCqrs(this Container container, IEnumerable<Assembly> assemblies)
        {
            RegisterCqrsCore(container)
                .RegisterCommandHandlers(assemblies)
                .RegisterCommandHandlersAttributes(assemblies)
                .RegisterEventHandlers(assemblies)
                .RegisterEventHandlersAttributes(assemblies);

            return container;
        }

        public static ICqrsBuilder RegisterCqrsCore(this Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            
            // Builder will search in entry and reference assemblies if no assembly is provided.
            return new CqrsBuilder(container)
                .RegisterCommandDelegator()
                .RegisterEventDelegator();
        }
    }
}
