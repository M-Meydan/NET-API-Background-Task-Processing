using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly ILogger<RequestReplyController> _logger;
        public RequestReplyController(IBackgroundMediator backgroundMediator, ILogger<RequestReplyController> logger)
        {
            _backgroundMediator = backgroundMediator;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> RequestReplyTask([FromBody] string taskInfo)
        {
            _logger.LogInformation("Starting RequestReplyTask()");

            _logger.LogInformation(" Fired RequestReplyTask command.");
            var command = new RequestReplyCommand { TaskInfo = taskInfo };
            var result = _backgroundMediator.SendAsync(command);

            _logger.LogInformation(" Start: Similating 1sec DB operation...");
            await Task.Delay(1000);
            _logger.LogInformation(" End: DB operation...");

            try
            {
                var awatedResult = await result;
            }catch(Exception ex)
            {
            }

            return Ok(await result);
        }
    }

}
