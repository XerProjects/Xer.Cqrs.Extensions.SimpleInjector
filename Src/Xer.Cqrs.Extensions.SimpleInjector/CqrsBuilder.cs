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
        private readonly IServiceProvider _serviceProvider;
        private readonly CqrsCommandHandlerSelector _commandHandlerFinder;
        private readonly CqrsEventHandlerSelector _eventHandlerFinder;

        public CqrsBuilder(Container container, IEnumerable<Assembly> assemblies)
        {
            _container = container;
            // Cast to IServiceProvider so that container will not throw if type is not registered.
            _serviceProvider = container;
            _commandHandlerFinder = new CqrsCommandHandlerSelector(container, assemblies);
            _eventHandlerFinder = new CqrsEventHandlerSelector(container, assemblies);
        }

        public ICqrsBuilder RegisterCommandHandlers(Action<ICqrsCommandHandlerSelector> filter = null)
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

            if (filter == null)
            {
                // Register all.
                _commandHandlerFinder.ByInterface().ByAttribute();
            }
            else
            {
                filter(_commandHandlerFinder);
            }

            return this;
        }

        public ICqrsBuilder RegisterEventHandlers(Action<ICqrsEventHandlerSelector> filter = null)
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

            if (filter == null)
            {
                // Register all.
                _eventHandlerFinder.ByInterface().ByAttribute();
            }
            else
            {
                filter(_eventHandlerFinder);
            }

            return this;
        }

        private bool tryGetInstance<T>(out T instance) where T : class
        {
            instance = _serviceProvider.GetService(typeof(T)) as T;
            if (instance == null)
            {
                return false;
            }

            return true;
        }
    }
}