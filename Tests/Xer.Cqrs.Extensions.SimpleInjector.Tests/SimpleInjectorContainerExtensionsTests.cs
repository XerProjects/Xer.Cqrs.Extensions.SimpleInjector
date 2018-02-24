using System;
using System.Threading.Tasks;
using FluentAssertions;
using SimpleInjector;
using Tests.Entities.CommandHandlers;
using Tests.Entities.EventHandlers;
using Xer.Cqrs.CommandStack;
using Xer.Cqrs.EventStack;
using Xer.Cqrs.Extensions.SimpleInjector;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class SimpleInjectorContainerExtensionsTests
    {
        public class AddCqrsMethods
        {
            private readonly System.Reflection.Assembly _handlerAssembly = typeof(TestCommand).Assembly;
            private readonly ITestOutputHelper _outputHelper;

            public AddCqrsMethods(ITestOutputHelper outputHelper)
            {
                _outputHelper = outputHelper;  
            }

            [Fact]
            public void ShouldRegisterAllCommandHandlersAndEventHandlersInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrs(_handlerAssembly);
                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var commandHandlerResolvers = container.GetAllInstances<CommandHandlerDelegateResolver>();
                var eventHandlerResolvers = container.GetAllInstances<EventHandlerDelegateResolver>();

                // Two Resolvers:
                // 1. Command handler resolver
                // 2. Command handler attribute resolver
                commandHandlerResolvers.Should().HaveCount(2);
                
                // Two Resolvers:
                // 1. Event handler resolver
                // 2. Event handler attribute resolver
                eventHandlerResolvers.Should().HaveCount(2);

                container.GetInstance<CommandDelegator>().Should().NotBeNull();
                container.GetInstance<EventDelegator>().Should().NotBeNull();
                container.GetInstance<TestCommandHandler>().Should().NotBeNull();
                container.GetInstance<TestEventHandler>().Should().NotBeNull();
                container.GetInstance<ICommandAsyncHandler<TestCommand>>().Should().NotBeNull();
                container.GetInstance<ICommandHandler<TestCommand>>().Should().NotBeNull();
                container.GetInstance<IEventAsyncHandler<TestEvent>>().Should().NotBeNull();
                container.GetInstance<IEventHandler<TestEvent>>().Should().NotBeNull();
            }

            [Fact]
            public void ShouldRegisterAllCommandHandlersInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore(_handlerAssembly)
                         .RegisterCommandHandlers(select => select.ByInterface(Lifestyle.Transient));

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<CommandHandlerDelegateResolver>();

                resolvers.Should().HaveCount(1);
                
                container.GetInstance<CommandDelegator>().Should().NotBeNull();
                container.GetInstance<ICommandAsyncHandler<TestCommand>>().Should().NotBeNull();
                container.GetInstance<ICommandHandler<TestCommand>>().Should().NotBeNull();
            }

            [Fact]
            public void ShouldRegisterAllCommandHandlerAttributesInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore(_handlerAssembly)
                         .RegisterCommandHandlers(select => select.ByAttribute(Lifestyle.Transient));
                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<CommandHandlerDelegateResolver>();

                resolvers.Should().HaveCount(1);

                container.GetInstance<CommandDelegator>().Should().NotBeNull();
                container.GetInstance<TestCommandHandler>().Should().NotBeNull();

                Action action = () => container.GetInstance<TestEventHandler>();
                action.Should().Throw<Exception>("because command handlers were not registered."); // Why? This should throw.
            }

            [Fact]
            public void ShouldRegisterAllEventHandlersInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore(_handlerAssembly)
                         .RegisterEventHandlers(select => select.ByInterface(Lifestyle.Transient));

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<EventHandlerDelegateResolver>();

                resolvers.Should().HaveCount(1);
                
                container.GetInstance<EventDelegator>().Should().NotBeNull();
                container.GetInstance<IEventAsyncHandler<TestEvent>>().Should().NotBeNull();
                container.GetInstance<IEventHandler<TestEvent>>().Should().NotBeNull();
            }

            [Fact]
            public void ShouldRegisterAllEventHandlerAttributesInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore(_handlerAssembly)
                         .RegisterEventHandlers(select => select.ByAttribute(Lifestyle.Transient));

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<EventHandlerDelegateResolver>();

                resolvers.Should().HaveCount(1);
                
                container.GetInstance<EventDelegator>().Should().NotBeNull();
                container.GetInstance<TestEventHandler>().Should().NotBeNull();

                Action action = () => container.GetInstance<TestCommandHandler>();
                action.Should().Throw<Exception>("because command handlers were not registered."); // Why? This should throw.
            }
        }
    }
}
