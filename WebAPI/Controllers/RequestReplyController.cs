using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebAPI.Mediator.Tasks.Commands;
using WebAPI.Services.TaskServices;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestReplyController : ControllerBase
    {
        private readonly IBackgroundMediator _backgroundMediator;
        private readonly ILogger<BackgroundTaskService> _logger;
        public RequestReplyController(IBackgroundMediator backgroundMediator, ILogger<BackgroundTaskService> logger)
        {
            _backgroundMediator = backgroundMediator;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartRequestReplyTask([FromBody] string taskInfo)
        {
            var command = new RequestReplyCommand { TaskInfo = taskInfo };
            var result = _backgroundMediator.SendAsync(command);

            _logger.LogInformation("API Start tasks:1 ...");

            await Task.Delay(1000);

            _logger.LogInformation("API Finished tasks:1");

            return Ok(await result);
        }
    }

}
