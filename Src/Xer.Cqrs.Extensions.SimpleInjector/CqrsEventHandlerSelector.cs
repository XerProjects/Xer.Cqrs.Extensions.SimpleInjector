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

        public ICqrsEventHandlerSelector ByInterface(params Assembly[] assemblies)
        {
            return ByInterface(Lifestyle.Transient, assemblies);
        }

        public ICqrsEventHandlerSelector ByInterface(Lifestyle lifeStyle, params Assembly[] assemblies)
        {
            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            IEnumerable<Assembly> distinctAssemblies = assemblies.Distinct();

            _container.Register(typeof(IEventAsyncHandler<>), distinctAssemblies, lifeStyle);
            _container.Register(typeof(IEventHandler<>), distinctAssemblies, lifeStyle);
            
            // Register resolver.
            _container.Collections.AppendTo(
                typeof(EventHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new EventHandlerDelegateResolver(new ContainerEventHandlerResolver(new SimpleInjectorContainerAdapter(_container))),
                    _container
                )
            );

            return this;
        }

        public ICqrsEventHandlerSelector ByAttribute(params Assembly[] assemblies)
        {
            return ByAttribute(Lifestyle.Transient, assemblies);
        }

        public ICqrsEventHandlerSelector ByAttribute(Lifestyle lifeStyle, params Assembly[] assemblies)
        {
            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            
            // Get all types that has methods marked with [EventHandler] attribute from distinct assemblies.
            IEnumerable<Type> allTypes = assemblies.Distinct()
                                                   .SelectMany(assembly => assembly.GetTypes())
                                                   .Where(type => type.IsClass &&
                                                                  !type.IsAbstract &&
                                                                  EventHandlerAttributeMethod.IsFoundInType(type))
                                                   .ToArray();

            foreach (Type type in allTypes)
            {
                // Register type as self.
                _container.Register(type, type, lifeStyle);
            }
            
            var multiMessageHandlerRegistration = new MultiMessageHandlerRegistration();
            multiMessageHandlerRegistration.RegisterEventHandlerAttributes(allTypes, _container.GetInstance);

            // Register resolver.
            _container.Collections.AppendTo(
                typeof(EventHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new EventHandlerDelegateResolver(multiMessageHandlerRegistration.BuildMessageHandlerResolver()),
                    _container
                )
            );

            return this;
        }
    }
}