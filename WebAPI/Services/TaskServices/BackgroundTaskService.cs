using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Concurrent;
using Utilities;

namespace WebAPI.Services.TaskServices
{
    /// <summary>
    /// The Hosted Service is responsible for continuously listening to the Channel and processing tasks in the background. 
    /// It only starts processing after the API has fully started and handles graceful shutdown by stopping task processing when the application stops.
    /// </summary>
    public class BackgroundTaskService : BackgroundService
    {
        private readonly Channel<IBackgroundCommand> _commandChannel;
        private readonly IMediator _mediator;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<BackgroundTaskService> _logger;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(500);  // Limit concurrency 

        public BackgroundTaskService(
           Channel<IBackgroundCommand> commandChannel,
            IMediator mediator,
            IHostApplicationLifetime applicationLifetime,
            ILogger<BackgroundTaskService> logger)
        {
            _commandChannel = commandChannel;
            _mediator = mediator;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _applicationLifetime.ApplicationStarted.Register(async () =>
            {
                _logger.LogInformation("API fully started. Background command processing begins.");
                await StartProcessingCommands(stoppingToken);
            });

            _applicationLifetime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("API is stopping. Background command processing will stop.");
            });
        }

        private async Task StartProcessingCommands(CancellationToken stoppingToken)
        {
                await foreach (var backgroundCommand in _commandChannel.Reader.ReadAllAsync(stoppingToken))
                {
                    await _semaphore.WaitAsync(stoppingToken);  // Acquire semaphore before starting a new task

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            _logger.LogInformation($"Processing background command [{backgroundCommand.Id}]: {backgroundCommand.GetType().Name}");

                            // If the command is fire-and-forget
                            if (backgroundCommand is BackgroundCommand<Unit> fireAndForgetCommand)
                            {
                                await _mediator.Send(fireAndForgetCommand);  // Fire-and-forget command
                            }
                            else // If the command is request-reply
                            {
                                var result = await _mediator.Send(backgroundCommand);
                                backgroundCommand.SetResult(result);  // Set result for request-reply command
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                            backgroundCommand.SetException(ex);  // Set exception for failed commands
                        }
                        finally
                        {
                            _semaphore.Release();  // Release semaphore when done
                        }
                    }, stoppingToken);
                }
        }

    }

}
