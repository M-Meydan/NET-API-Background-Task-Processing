using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Services.TaskServices;

namespace WebAPI.Mediator.Tasks.Commands
{
    public class FireAndForgetCommand : BackgroundCommand<Unit>
    {
        public string TaskInfo { get; set; }

        public class FireAndForgetCommandHandler : IRequestHandler<FireAndForgetCommand, Unit>
        {
            private readonly ILogger<FireAndForgetCommandHandler> _logger;

            public FireAndForgetCommandHandler(ILogger<FireAndForgetCommandHandler> logger)
            {
                _logger = logger;
            }

            public async Task<Unit> Handle(FireAndForgetCommand command, CancellationToken cancellationToken)
            {
                int i = 1;
                if (i == 0)
                    throw new System.Exception("Simulated Exception");

                // Simulate some background work
                await System.Threading.Tasks.Task.Delay(3000);
                _logger.LogInformation($"   [{command.Id}] = Fire-and-Forget Command executing: {command.TaskInfo}");

                return Unit.Value;
            }
        }
    }


}
