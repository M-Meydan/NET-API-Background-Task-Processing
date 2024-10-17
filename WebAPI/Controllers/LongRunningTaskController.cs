using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebAPI.Mediator.Tasks.Commands;
using WebAPI.Mediator.Tasks.Queries;
using WebAPI.Models;
using WebAPI.Services.TaskServices;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongRunningTaskController : ControllerBase
    {
        private readonly ILogger<LongRunningTaskController> _logger;
       
        private readonly IBackgroundMediator _backgroundMediator;
        private readonly IMediator _mediator;

        public LongRunningTaskController(IBackgroundMediator backgroundMediator, IMediator mediator, ILogger<LongRunningTaskController> logger)
        {
            _backgroundMediator = backgroundMediator;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> LongRunningTask([FromBody] LongRunningTaskRequest request)
        {
            _logger.LogInformation("Starting LongRunningTask()");

            _logger.LogInformation(" Fired LongRunningTaskCommand command.");
            var command = new LongRunningTaskCommand(request.TaskInfo, Guid.NewGuid(), request.SimulateError);
            await _backgroundMediator.SendAsync(command);

            _logger.LogInformation("Finished LongRunningTask()");

            // Return the TaskId for querying the progress
            return Ok(new { TaskId = command.TaskId });
        }

        [HttpGet("progress/{taskId}")]
        public async Task<IActionResult> GetTaskProgressAsync(Guid taskId)
        {
            var progressQuery = new GetTaskProgressQuery(taskId);
            var progress = await _mediator.Send(progressQuery);

            return Ok(progress);
        }
    }

}
