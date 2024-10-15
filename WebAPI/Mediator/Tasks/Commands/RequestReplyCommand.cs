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
        public async Task<string> Handle(RequestReplyCommand request, CancellationToken cancellationToken)
        {
            // Simulate some background work
            await Task.Delay(2000);
            _logger.LogInformation($"-- Request-Reply Command executing in the background: {request.TaskInfo}");
            return $"Task Completed: {request.TaskInfo}";
        }
    }

}
