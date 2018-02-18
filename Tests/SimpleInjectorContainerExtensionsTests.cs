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
            public async Task ShouldRegisterAllCommandHandlersAndEventHandlersInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrs(_handlerAssembly);
                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var commandHandlerResolvers = container.GetAllInstances<CommandHandlerDelegateResolver>();
                var eventHandlerResolvers = container.GetAllInstances<EventHandlerDelegateResolver>();
                var commandDelegator = container.GetInstance<CommandDelegator>();
                var eventDelegator = container.GetInstance<EventDelegator>();

                // Two Resolvers:
                // 1. Command handler resolver
                // 2. Command handler attribute resolver
                commandHandlerResolvers.Should().HaveCount(2);
                
                // Two Resolvers:
                // 1. Event handler resolver
                // 2. Event handler attribute resolver
                eventHandlerResolvers.Should().HaveCount(2);

                await commandDelegator.SendAsync(new TestCommand());
                await eventDelegator.SendAsync(new TestEvent());
            }

            [Fact]
            public async Task ShouldRegisterAllCommandHandlersInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore()
                         .RegisterCommandHandlers(_handlerAssembly);

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<CommandHandlerDelegateResolver>();
                var commandDelegator = container.GetInstance<CommandDelegator>();

                resolvers.Should().HaveCount(1);

                await commandDelegator.SendAsync(new TestCommand());
            }

            [Fact]
            public async Task ShouldRegisterAllCommandHandlerAttributesInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore()
                         .RegisterCommandHandlersAttributes(_handlerAssembly);

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<CommandHandlerDelegateResolver>();
                var commandDelegator = container.GetInstance<CommandDelegator>();

                resolvers.Should().HaveCount(1);

                await commandDelegator.SendAsync(new TestCommand());
            }

            [Fact]
            public async Task ShouldRegisterAllEventHandlersInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore()
                         .RegisterEventHandlers(_handlerAssembly);

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<EventHandlerDelegateResolver>();
                var eventDelegator = container.GetInstance<EventDelegator>();

                resolvers.Should().HaveCount(1);

                await eventDelegator.SendAsync(new TestEvent());
            }

            [Fact]
            public async Task ShouldRegisterAllEventHandlerAttributesInAssembly()
            {
                Container container = new Container();
                container.RegisterCqrsCore()
                         .RegisterEventHandlersAttributes(_handlerAssembly);

                container.RegisterSingleton(_outputHelper);
                container.Verify();

                var resolvers = container.GetAllInstances<EventHandlerDelegateResolver>();
                var eventDelegator = container.GetInstance<EventDelegator>();

                resolvers.Should().HaveCount(1);

                await eventDelegator.SendAsync(new TestEvent());
            }
        }
    }
}
