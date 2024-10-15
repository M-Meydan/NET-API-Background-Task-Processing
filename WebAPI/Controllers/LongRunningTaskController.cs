using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using System.Threading.Tasks;
using System;
using WebAPI.Mediator.Tasks.Commands;
using WebAPI.Services.TaskServices;
using Microsoft.Extensions.Logging;
using WebAPI.Mediator.Tasks.Queries;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongRunningTaskController : ControllerBase
    {
        private readonly ILogger<FireAndForgetController> _logger;
       
        private readonly IBackgroundMediator _backgroundMediator;
        private readonly IMediator _mediator;

        public LongRunningTaskController(IBackgroundMediator backgroundMediator, IMediator mediator)
        {
            _backgroundMediator = backgroundMediator;
            _mediator = mediator;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartLongRunningTask([FromBody] LongRunningTaskRequest request)
        {
            var command = new LongRunningTaskCommand
            {
                TaskInfo = request.TaskInfo,
                TaskId = Guid.NewGuid(),  
                SimulateError = request.SimulateError
            };

            await _backgroundMediator.SendAsync(command);

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
