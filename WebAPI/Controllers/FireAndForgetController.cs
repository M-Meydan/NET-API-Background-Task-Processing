using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebAPI.Mediator.Tasks.Commands;
using WebAPI.Services.TaskServices;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FireAndForgetController : ControllerBase
    { 
        private readonly ILogger<FireAndForgetController> _logger;
        private readonly IBackgroundMediator _backgroundMediator;
        public FireAndForgetController(Channel<IBackgroundCommand> commandChannel, ILogger<FireAndForgetController> logger, IBackgroundMediator backgroundMediator)
        {
            _logger = logger;
            _backgroundMediator = backgroundMediator;
        }

        [HttpPost("start")]
        public async Task<IActionResult> FireAndForgetTask([FromBody] string taskInfo)
        {
            _logger.LogInformation("Starting FireAndForgetTask()");

            var command = new FireAndForgetCommand { TaskInfo = taskInfo };
            await _backgroundMediator.SendAsync(command);
            _logger.LogInformation($" Fired [{command.Id}] Fire-and-Forget command.");

            _logger.LogInformation("Finished FireAndForgetTask()");
            return Ok(taskInfo);
        }
    }

}
