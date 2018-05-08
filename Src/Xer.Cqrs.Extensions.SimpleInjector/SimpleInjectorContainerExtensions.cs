using System;
using System.Reflection;
using Xer.Cqrs.Extensions.SimpleInjector;

namespace SimpleInjector
{
    public static class SimpleInjectorContainerExtensions
    {
        public static Container RegisterCqrs(this Container container, params Assembly[] assemblies)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentException("No assemblies were provided.", nameof(assemblies));
            }
            
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
