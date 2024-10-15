namespace WebAPI.Models
{
    public class LongRunningTaskRequest
    {
        public string TaskInfo { get; set; }
        public bool SimulateError { get; set; } = false; // Default to false
    }
}
