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

        public ICqrsCommandHandlerSelector ByInterface(params Assembly[] assemblies)
        {
            return ByInterface(Lifestyle.Transient, assemblies);
        }

        public ICqrsCommandHandlerSelector ByInterface(Lifestyle lifeStyle, params Assembly[] assemblies)
        {
            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }
            
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentException("No assemblies were provided.", nameof(assemblies));
            }

            IEnumerable<Assembly> distinctAssemblies = assemblies.Distinct();

            _container.Register(typeof(ICommandAsyncHandler<>), distinctAssemblies, lifeStyle);
            _container.Register(typeof(ICommandHandler<>), distinctAssemblies, lifeStyle);
            
            // Register resolver.
            _container.Collections.AppendTo(
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

        public ICqrsCommandHandlerSelector ByAttribute(params Assembly[] assemblies)
        {
            return ByAttribute(Lifestyle.Transient, assemblies);
        }

        public ICqrsCommandHandlerSelector ByAttribute(Lifestyle lifeStyle, params Assembly[] assemblies)
        {
            if (lifeStyle == null)
            {
                throw new ArgumentNullException(nameof(lifeStyle));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentException("No assemblies were provided.", nameof(assemblies));
            }
            
            // Get all types that has methods marked with [CommandHandler] attribute from distinct assemblies.
            IEnumerable<Type> allTypes = assemblies.Distinct()
                                                   .SelectMany(assembly => assembly.GetTypes())
                                                   .Where(type => type.IsClass &&
                                                                  !type.IsAbstract &&
                                                                  CommandHandlerAttributeMethod.IsFoundInType(type))
                                                   .ToArray();

            foreach (Type type in allTypes)
            {
                // Register type as self.
                _container.Register(type, type, lifeStyle);
            }

            var singleMessageHandlerRegistration = new SingleMessageHandlerRegistration();
            singleMessageHandlerRegistration.RegisterCommandHandlerAttributes(allTypes, _container.GetInstance);

            // Register resolver.
            _container.Collections.AppendTo(
                typeof(CommandHandlerDelegateResolver),
                Lifestyle.Singleton.CreateRegistration(() =>
                    new CommandHandlerDelegateResolver(singleMessageHandlerRegistration.BuildMessageHandlerResolver()),
                    _container
                )
            );

            return this;
        }
    }
}