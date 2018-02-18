using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;
using Xer.Cqrs.CommandStack;
using Xer.Cqrs.CommandStack.Attributes;
using Xer.Cqrs.CommandStack.Resolvers;
using Xer.Cqrs.EventStack;
using Xer.Cqrs.EventStack.Attributes;
using Xer.Cqrs.EventStack.Resolvers;
using Xer.Delegator;
using Xer.Delegator.Registrations;
using Xer.Delegator.Resolvers;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    internal class CqrsBuilder : ICqrsBuilder
    {
        private readonly Container _container;

        public CqrsBuilder(Container container)
        {
            _container = container;
        }

        public ICqrsBuilder RegisterCommandDelegator()
        {
            _container.RegisterSingleton<CommandDelegator>(() =>
            {
                if (tryGetInstance(out IEnumerable<CommandHandlerDelegateResolver> commandHandlerResolvers))
                {
                    CommandHandlerDelegateResolver[] resolverArray = commandHandlerResolvers.ToArray();

                    if (resolverArray.Length == 1)
                    {
                        return new CommandDelegator(resolverArray[0]);
                    }

                    return new CommandDelegator(CompositeMessageHandlerResolver.Compose(resolverArray));
                }
                
                return new CommandDelegator(new SingleMessageHandlerRegistration().BuildMessageHandlerResolver());
            });

            return this;
        }

        public ICqrsBuilder RegisterCommandHandlers(Assembly assembly)
        {
            return RegisterCommandHandlers(assembly, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterCommandHandlers(Assembly assembly, Lifestyle lifeStyle)
        {
            return RegisterCommandHandlers(new[] { assembly }, lifeStyle);
        }

        public ICqrsBuilder RegisterCommandHandlers(IEnumerable<Assembly> assemblies)
        {
            return RegisterCommandHandlers(assemblies, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterCommandHandlers(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
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

        public ICqrsBuilder RegisterCommandHandlersAttributes(Assembly assembly)
        {
            return RegisterCommandHandlersAttributes(assembly, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterCommandHandlersAttributes(Assembly assembly, Lifestyle lifeStyle)
        {
            return RegisterCommandHandlersAttributes(new[] { assembly }, lifeStyle);
        }

        public ICqrsBuilder RegisterCommandHandlersAttributes(IEnumerable<Assembly> assemblies)
        {
            return RegisterCommandHandlersAttributes(assemblies, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterCommandHandlersAttributes(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
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

        public ICqrsBuilder RegisterEventDelegator()
        {
            _container.RegisterSingleton<EventDelegator>(() =>
            {                
                if (tryGetInstance(out IEnumerable<EventHandlerDelegateResolver> eventHandlerResolvers))
                {
                    EventHandlerDelegateResolver[] resolverArray = eventHandlerResolvers.ToArray();

                    if (resolverArray.Length == 1)
                    {
                        return new EventDelegator(resolverArray[0]);
                    }

                    return new EventDelegator(CompositeMessageHandlerResolver.Compose(resolverArray));
                }

                return new EventDelegator(new SingleMessageHandlerRegistration().BuildMessageHandlerResolver());
            });

            return this;
        }

        public ICqrsBuilder RegisterEventHandlers(Assembly assembly)
        {
            return RegisterEventHandlers(assembly, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterEventHandlers(Assembly assembly, Lifestyle lifeStyle)
        {
            return RegisterEventHandlers(new[] { assembly }, lifeStyle);
        }

        public ICqrsBuilder RegisterEventHandlers(IEnumerable<Assembly> assemblies)
        {
            return RegisterEventHandlers(assemblies, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterEventHandlers(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
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

        public ICqrsBuilder RegisterEventHandlersAttributes(Assembly assembly)
        {
            return RegisterEventHandlersAttributes(assembly, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterEventHandlersAttributes(Assembly assembly, Lifestyle lifeStyle)
        {
            return RegisterEventHandlersAttributes(new[] { assembly }, lifeStyle);
        }

        public ICqrsBuilder RegisterEventHandlersAttributes(IEnumerable<Assembly> assemblies)
        {
            return RegisterEventHandlersAttributes(assemblies, Lifestyle.Transient);
        }

        public ICqrsBuilder RegisterEventHandlersAttributes(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle)
        {
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

        private bool tryGetInstance<T>(out T instance) where T : class
        {
            // Cast to IServiceProvider so that container will not throw if type is not registered.
            IServiceProvider serviceProvider = _container;

            instance = serviceProvider.GetService(typeof(T)) as T;
            if (instance == null)
            {
                return false;
            }

            return true;
        }
    }
}