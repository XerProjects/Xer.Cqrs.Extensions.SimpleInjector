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
        private readonly CqrsCommandHandlerSelector _commandHandlerSelector;
        private readonly CqrsEventHandlerSelector _eventHandlerSelector;

        internal CqrsBuilder(Container container)
        {
            _container = container;
            // Cast to IServiceProvider so that container will not throw if type is not registered.
            _serviceProvider = container;
            _commandHandlerSelector = new CqrsCommandHandlerSelector(container);
            _eventHandlerSelector = new CqrsEventHandlerSelector(container);
        }

        public ICqrsBuilder RegisterCommandHandlers(Action<ICqrsCommandHandlerSelector> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            selector.Invoke(_commandHandlerSelector);

            _container.RegisterSingleton<CommandDelegator>(() =>
            {
                if (TryGetInstance(out IEnumerable<CommandHandlerDelegateResolver> commandHandlerResolvers))
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

        public ICqrsBuilder RegisterEventHandlers(Action<ICqrsEventHandlerSelector> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            
            selector(_eventHandlerSelector);

            _container.RegisterSingleton<EventDelegator>(() =>
            {                
                if (TryGetInstance(out IEnumerable<EventHandlerDelegateResolver> eventHandlerResolvers))
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

        private bool TryGetInstance<T>(out T instance) where T : class
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