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
        private readonly IEnumerable<Assembly> _assemblies;

        internal CqrsEventHandlerSelector(Container container, IEnumerable<Assembly> assemblies)
        {
            _container = container;
            _assemblies = assemblies;
        }

        public ICqrsEventHandlerSelector ByAttribute()
        {
            return ByAttribute(Lifestyle.Transient);
        }

        public ICqrsEventHandlerSelector ByAttribute(Lifestyle lifeStyle)
        {
            // Get all types that has methods marked with [EventHandler] attribute.
            IEnumerable<Type> allTypes = _assemblies.SelectMany(assembly => assembly.GetTypes())
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

        public ICqrsEventHandlerSelector ByInterface()
        {
            return ByInterface(Lifestyle.Transient);
        }

        public ICqrsEventHandlerSelector ByInterface(Lifestyle lifeStyle)
        {
            _container.Register(typeof(IEventAsyncHandler<>), _assemblies, lifeStyle);
            _container.Register(typeof(IEventHandler<>), _assemblies, lifeStyle);
            
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