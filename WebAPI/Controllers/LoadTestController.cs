using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoadTestController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _localhost = "https://localhost:5000";
        public LoadTestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("fire-forget")]
        public async Task<IActionResult> StartFireAndForgetLoadTest(int numberOfRequests)
        {
            var client = _httpClientFactory.CreateClient();
            var tasks = new Task[numberOfRequests];

            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks[i] = client.PostAsJsonAsync($"{_localhost}/api/FireAndForget/start", new { TaskInfo = $"Test {i}" });
            }

            await Task.WhenAll(tasks);
            return Ok(new { message = $"{numberOfRequests} Fire-and-Forget requests sent." });
        }

        [HttpPost("long-running")]
        public async Task<IActionResult> StartLongRunningTaskLoadTest(int numberOfRequests)
        {
            var client = _httpClientFactory.CreateClient();
            var tasks = new Task[numberOfRequests];

            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks[i] = client.PostAsJsonAsync($"{_localhost}/api/LongRunningTask/start", new { TaskInfo = $"Test {i}", SimulateError = false });
            }

            await Task.WhenAll(tasks);
            return Ok(new { message = $"{numberOfRequests} Long-Running requests sent." });
        }

        [HttpPost("request-reply")]
        public async Task<IActionResult> StartRequestReplyLoadTest(int numberOfRequests)
        {
            var client = _httpClientFactory.CreateClient();
            var tasks = new Task[numberOfRequests];

            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks[i] = client.PostAsJsonAsync($"{_localhost}/api/RequestReply/start", new { TaskInfo = $"Test {i}" });
            }

            await Task.WhenAll(tasks);
            return Ok(new { message = $"{numberOfRequests} Request-Reply requests sent." });
        }
    }
}