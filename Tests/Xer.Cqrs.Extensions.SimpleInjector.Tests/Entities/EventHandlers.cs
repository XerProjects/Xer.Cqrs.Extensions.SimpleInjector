using System.Threading;
using System.Threading.Tasks;
using Xer.Cqrs.EventStack;
using Xer.Cqrs.EventStack.Extensions.Attributes;
using Xunit.Abstractions;

namespace Tests.Entities.EventHandlers
{
    public class TestEvent { }

    public class TestEventHandler : IEventAsyncHandler<TestEvent>,
                                    IEventHandler<TestEvent>
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestEventHandler(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            _outputHelper.WriteLine($"{GetType().Name} handled {@event.GetType().Name} event asynchronously.");
            return Task.CompletedTask;
        }
        
        public void Handle(TestEvent @event)
        {
            _outputHelper.WriteLine($"{GetType().Name} handled {@event.GetType().Name} event synchronously.");
        }

        [EventHandler] // To allow method to be registered through attribute registration.
        public Task HandleTestEventAsync(TestEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            _outputHelper.WriteLine($"{GetType().Name} handled {@event.GetType().Name} event in EventHandler method.");
            return Task.CompletedTask;
        }
    }
}