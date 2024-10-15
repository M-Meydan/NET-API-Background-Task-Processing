using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPI.Models
{
    /// <summary>
    /// Represents the progress and status of a long-running task.
    /// </summary>
    public class TaskProgressResponse
    {
        /// <summary>
        /// Unique identifier for the task, useful for tracking progress.
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// Name or description of the task to provide context for tracking.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Custom status message that can display the current state of the task, 
        /// such as "Processing", "Waiting", "Completed", or "Error".
        /// </summary>
        public List<string> StatusMessages { get; set; }

        /// <summary>
        /// The percentage of task completion, ranging from 0 to 100.
        /// </summary>
        public double PercentageComplete { get; set; }

        /// <summary>
        /// The time when the task started, used to calculate elapsed time or estimate completion time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Optional field representing the estimated time for task completion, 
        /// can be set if estimations are needed based on the task's progress.
        /// </summary>
        public DateTime? EstimatedCompletionTime { get; set; }

        /// <summary>
        /// The actual time when the task was completed. 
        /// This is only set once the task finishes, either successfully or unsuccessfully.
        /// </summary>
        public DateTime? ActualCompletionTime { get; set; }

        /// <summary>
        /// Indicates whether the task has been completed.
        /// This is set to true once the task reaches 100% or encounters a terminal error.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Indicates whether the task was completed successfully. 
        /// This is set to false if the task encountered any critical errors.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// A list of error messages encountered during task execution, 
        /// useful for logging issues or debugging failures.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Constructor to initialize an empty error list when the task is created.
        /// </summary>
        public TaskProgressResponse(Guid taskId)
        {
            TaskId = taskId;
            StatusMessages = new List<string>();
            Errors = new List<string>(); 
        }
    }

}
