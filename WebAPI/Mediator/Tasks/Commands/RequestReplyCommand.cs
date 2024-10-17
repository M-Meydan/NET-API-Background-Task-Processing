using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Services.TaskServices;

namespace WebAPI.Mediator.Tasks.Commands
{
    public class RequestReplyCommand : BackgroundCommand<string>
    {
        public string TaskInfo { get; set; }

    }
    public class RequestReplyCommandHandler : IRequestHandler<RequestReplyCommand, string>
    {
        private readonly ILogger<RequestReplyCommandHandler> _logger;

        public RequestReplyCommandHandler(ILogger<RequestReplyCommandHandler> logger)
        {
            _logger = logger;
        }
        public async Task<string> Handle(RequestReplyCommand command, CancellationToken cancellationToken)
        {
            int i = 1;
            if (i == 0)
                throw new System.Exception("Simulated Exception");

            // Simulate some background work
            await Task.Delay(3000);
            _logger.LogInformation($"   [{command.Id}] = Request-Reply Command executing in the background: {command.TaskInfo}");
            return $"Task Completed: {command.TaskInfo}";
        }
    }

}
