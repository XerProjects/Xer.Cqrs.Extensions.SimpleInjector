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

        internal CqrsCommandHandlerSelector(Container container)
        {
            _container = container;
        }

        public ICqrsCommandHandlerSelector ByAttribute(Assembly assembly)
        {
            return ByAttribute(assembly, Lifestyle.Transient);
        }

        public ICqrsCommandHandlerSelector ByAttribute(Assembly assembly, Lifestyle lifestyle)
        {
            return ByAttribute(new[] { assembly }, lifestyle);
        }

        public ICqrsCommandHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies)
        {
            return ByAttribute(assemblies, Lifestyle.Transient);
        }

        public ICqrsCommandHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }
            
            // Get all types that has methods marked with [CommandHandler] attribute.
            IEnumerable<Type> allTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
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

        public ICqrsCommandHandlerSelector ByInterface(Assembly assembly)
        {
            return ByInterface(assembly, Lifestyle.Transient);
        }

        public ICqrsCommandHandlerSelector ByInterface(Assembly assembly, Lifestyle lifeStyle)
        {
            return ByInterface(new[] { assembly }, lifeStyle);
        }

        public ICqrsCommandHandlerSelector ByInterface(IEnumerable<Assembly> assemblies)
        {
            return ByInterface(assemblies, Lifestyle.Transient);
        }

        public ICqrsCommandHandlerSelector ByInterface(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }

            _container.Register(typeof(ICommandAsyncHandler<>), assemblies, lifeStyle);
            _container.Register(typeof(ICommandHandler<>), assemblies, lifeStyle);
            
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