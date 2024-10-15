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

            public async Task<Unit> Handle(FireAndForgetCommand request, CancellationToken cancellationToken)
            {
                // Simulate some background work
                await System.Threading.Tasks.Task.Delay(2000);
                _logger.LogInformation($"-- Fire-and-Forget Command executing in the background: {request.TaskInfo}");

                // Return Unit.Value to signify completion
                return Unit.Value;
            }
        }
    }


}
