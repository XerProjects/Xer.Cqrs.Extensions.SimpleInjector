using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;
using Xer.Cqrs.EventStack;
using Xer.Cqrs.EventStack.Resolvers;
using Xer.Delegator.Registrations;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    internal class CqrsEventHandlerSelector : ICqrsEventHandlerSelector
    {
        private readonly Container _container;

        internal CqrsEventHandlerSelector(Container container)
        {
            _container = container;
        }

        public ICqrsEventHandlerSelector ByAttribute(Assembly assembly)
        {
            return ByAttribute(assembly, Lifestyle.Transient);
        }

        public ICqrsEventHandlerSelector ByAttribute(Assembly assembly, Lifestyle lifestyle)
        {
            return ByAttribute(new[] { assembly }, lifestyle);
        }

        public ICqrsEventHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies)
        {
            return ByAttribute(assemblies, Lifestyle.Transient);
        }

        public ICqrsEventHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }
            
            // Get all types that has methods marked with [EventHandler] attribute.
            IEnumerable<Type> allTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                                                   .Where(type => type.IsClass &&
                                                                  !type.IsAbstract &&
                                                                  EventHandlerAttributeMethod.IsFoundInType(type)).ToArray();

            foreach(Type type in allTypes)
            {
                // Register type as self.
                _container.Register(type, type, lifeStyle);
            }
            
            var multiMessageHandlerRegistration = new MultiMessageHandlerRegistration();
            multiMessageHandlerRegistration.RegisterEventHandlerAttributes(allTypes, _container.GetInstance);

            // Register resolver.
            _container.AppendToCollection(
                typeof(EventHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new EventHandlerDelegateResolver(multiMessageHandlerRegistration.BuildMessageHandlerResolver()),
                    _container
                )
            );

            return this;
        }

        public ICqrsEventHandlerSelector ByInterface(Assembly assembly)
        {
            return ByInterface(assembly, Lifestyle.Transient);
        }

        public ICqrsEventHandlerSelector ByInterface(Assembly assembly, Lifestyle lifestyle)
        {
            return ByInterface(new[] { assembly }, lifestyle);
        }

        public ICqrsEventHandlerSelector ByInterface(IEnumerable<Assembly> assemblies)
        {
            return ByInterface(assemblies, Lifestyle.Transient);
        }

        public ICqrsEventHandlerSelector ByInterface(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }

            _container.Register(typeof(IEventAsyncHandler<>), assemblies, lifeStyle);
            _container.Register(typeof(IEventHandler<>), assemblies, lifeStyle);
            
            // Register resolver.
            _container.AppendToCollection(
                typeof(EventHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new EventHandlerDelegateResolver(new ContainerEventHandlerResolver(new SimpleInjectorContainerAdapter(_container))),
                    _container
                )
            );

            return this;
        }
    }
}