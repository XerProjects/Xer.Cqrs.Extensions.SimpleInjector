using System.Threading;
using System.Threading.Tasks;
using Xer.Cqrs.CommandStack;
using Xer.Cqrs.CommandStack.Extensions.Attributes;
using Xunit.Abstractions;

namespace Tests.Entities.CommandHandlers
{
    public class TestCommand { }
    
    public class TestCommandHandler : ICommandAsyncHandler<TestCommand>,
                                      ICommandHandler<TestCommand>
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestCommandHandler(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _outputHelper.WriteLine($"{GetType().Name} handled {command.GetType().Name} command asynchronously.");
            return Task.CompletedTask;
        }

        public void Handle(TestCommand command)
        {
            _outputHelper.WriteLine($"{GetType().Name} handled {command.GetType().Name} command synchronously.");
        }

        [CommandHandler] // To allow method to be registered through attribute registration.
        public Task HandleTestCommandAsync(TestCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _outputHelper.WriteLine($"{GetType().Name} handled {command.GetType().Name} command in CommandHandler method.");
            return Task.CompletedTask;
        }
    }
}