using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using WebAPI.Services.TaskServices;

namespace WebAPI.Mediator.Tasks.Commands
{
    public class LongRunningTaskCommand : BackgroundCommand<Unit>
    {
        public LongRunningTaskCommand(string taskInfo, Guid guid, bool simulateError)
        {
            TaskInfo = taskInfo;
            TaskId = guid;
            SimulateError = simulateError;
        }

        public Guid TaskId { get; set; } = Guid.NewGuid();
        public string TaskInfo { get; set; }
        public bool SimulateError { get; set; } = false;

        public class LongRunningTaskCommandHandler : IRequestHandler<LongRunningTaskCommand, Unit>
        {
            private readonly ITaskProgressService _taskProgressService;
            private readonly ILogger<LongRunningTaskCommandHandler> _logger;

            public LongRunningTaskCommandHandler(ITaskProgressService taskProgressService, ILogger<LongRunningTaskCommandHandler> logger)
            {
                _taskProgressService = taskProgressService;
                _logger = logger;
            }

            public async Task<Unit> Handle(LongRunningTaskCommand command, CancellationToken cancellationToken)
            {
                // Start tracking task progress
                _taskProgressService.StartTracking(command.TaskId, command.TaskInfo);

                try
                {
                    // Simulate long-running work with progress tracking
                    for (int i = 0; i <= 100; i += 20)
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Handle cancellation

                        await System.Threading.Tasks.Task.Delay(3000, cancellationToken); // Simulate work delay

                        // Report progress and append status message
                        var progressMessage = $"Task {command.TaskInfo} is {i}% complete";
                        _taskProgressService.ReportProgress(command.TaskId, i, progressMessage);

                        _logger.LogInformation(progressMessage);

                        // Simulate an error at 60% progress
                        if (i == 60 && command.SimulateError)
                        {
                            var errorMessage = $"Error occurred during {command.TaskInfo}";
                            _taskProgressService.ReportError(command.TaskId, errorMessage);
                            throw new Exception(errorMessage);
                        }
                    }

                    // Report task completion
                    _taskProgressService.ReportProgress(command.TaskId, 100, "Task Completed Successfully");
                    _logger.LogInformation($"Task {command.TaskInfo} completed successfully.");
                }
                catch (OperationCanceledException)
                {
                    // Handle task cancellation
                    _logger.LogWarning($"Task {command.TaskId} was canceled.");
                    _taskProgressService.ReportError(command.TaskId, "Task was canceled.");
                }
                catch (Exception ex)
                {
                    // Handle task error
                    _logger.LogError(ex, $"Task {command.TaskId} failed.");
                    _taskProgressService.ReportError(command.TaskId, $"Task failed: {ex.Message}");
                }

                return Unit.Value;
            }
        }
    }
}
