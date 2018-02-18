using System;
using System.Collections.Generic;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    /// <summary>
    /// Represents an adapter to SimpleInjector's Container. 
    /// This will request an instance from the container without throwing an exception
    /// if the requested type is not registered to the container.
    /// </summary>
    public class SimpleInjectorContainerAdapter : CommandStack.Resolvers.IContainerAdapter,
                                                  EventStack.Resolvers.IContainerAdapter
    {
        private readonly IServiceProvider _serviceProvider;

        public SimpleInjectorContainerAdapter(Container container)
        {
            // Cast to IServiceProvider so that container will not throw if type is not registered.
            _serviceProvider = container;
        }

        public T Resolve<T>() where T : class => _serviceProvider.GetService(typeof(T)) as T;
        public IEnumerable<T> ResolveMultiple<T>() where T : class => _serviceProvider.GetService(typeof(IEnumerable<T>)) as IEnumerable<T>;
    }
}