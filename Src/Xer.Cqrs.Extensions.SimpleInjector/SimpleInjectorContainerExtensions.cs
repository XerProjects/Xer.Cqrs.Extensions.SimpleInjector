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
            RegisterCqrsCore(container)
                .RegisterCommandHandlers(select => 
                    select.ByInterface(assemblies)
                          .ByAttribute(assemblies))
                .RegisterEventHandlers(select =>
                    select.ByInterface(assemblies)
                          .ByAttribute(assemblies));

            return container;
        }

        public static ICqrsBuilder RegisterCqrsCore(this Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return new CqrsBuilder(container);
        }
    }
}
