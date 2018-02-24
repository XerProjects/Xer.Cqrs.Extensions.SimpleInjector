using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;
using Xer.Cqrs.CommandStack;
using Xer.Cqrs.CommandStack.Resolvers;
using Xer.Delegator.Registrations;
using Xer.Delegator.Resolvers;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    internal class CqrsCommandHandlerSelector : ICqrsCommandHandlerSelector
    {
        private readonly Container _container;
        private readonly IEnumerable<Assembly> _assemblies;

        internal CqrsCommandHandlerSelector(Container container, IEnumerable<Assembly> assemblies)
        {
            _container = container;
            _assemblies = assemblies;
        }

        public ICqrsCommandHandlerSelector ByAttribute()
        {
            return ByAttribute(Lifestyle.Transient);
        }

        public ICqrsCommandHandlerSelector ByAttribute(Lifestyle lifeStyle)
        {
            // Get all types that has methods marked with [CommandHandler] attribute.
            IEnumerable<Type> allTypes = _assemblies.SelectMany(assembly => assembly.GetTypes())
                                                    .Where(type => type.IsClass &&
                                                                   !type.IsAbstract &&
                                                                   CommandHandlerAttributeMethod.IsFoundInType(type)).ToArray();

            foreach(Type type in allTypes)
            {
                // Register type as self.
                _container.Register(type, type, lifeStyle);
            }

            var singleMessageHandlerRegistration = new SingleMessageHandlerRegistration();
            singleMessageHandlerRegistration.RegisterCommandHandlerAttributes(allTypes, _container.GetInstance);

            // Register resolver.
            _container.AppendToCollection(
                typeof(CommandHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new CommandHandlerDelegateResolver(singleMessageHandlerRegistration.BuildMessageHandlerResolver()),
                    _container
                )
            );

            return this;
        }

        public ICqrsCommandHandlerSelector ByInterface()
        {
            return ByInterface(Lifestyle.Transient);
        }

        public ICqrsCommandHandlerSelector ByInterface(Lifestyle lifeStyle)
        {
            _container.Register(typeof(ICommandAsyncHandler<>), _assemblies, lifeStyle);
            _container.Register(typeof(ICommandHandler<>), _assemblies, lifeStyle);
            
            // Register resolver.
            _container.AppendToCollection(
                typeof(CommandHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new CommandHandlerDelegateResolver(
                        // Combine container async and sync command handler resolver.
                        CompositeMessageHandlerResolver.Compose(
                            new ContainerCommandAsyncHandlerResolver(new SimpleInjectorContainerAdapter(_container)),
                            new ContainerCommandHandlerResolver(new SimpleInjectorContainerAdapter(_container)))),
                    _container
                )
            );

            return this;
        }
    }
}