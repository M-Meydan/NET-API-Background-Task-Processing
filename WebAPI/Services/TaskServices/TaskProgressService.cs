using System;
using System.Collections.Concurrent;
using System.Linq;
using WebAPI.Models;

namespace WebAPI.Services.TaskServices
{
    public interface ITaskProgressService
    {
        TaskProgressResponse GetProgress(Guid taskId);
        void ReportError(Guid taskId, string errorMessage);
        void ReportProgress(Guid taskId, double percentage, string statusMessage = null);
        void StartTracking(Guid taskId, string taskName);
    }

    public class TaskProgressService : ITaskProgressService
    {
        private const int MaxTasks = 100;  // Maximum number of tasks allowed in progress tracking
        private readonly ConcurrentDictionary<Guid, TaskProgressResponse> _taskProgress;

        public TaskProgressService()
        {
            _taskProgress = new ConcurrentDictionary<Guid, TaskProgressResponse>();
        }

        // Start tracking a task, ensuring no more than 100 tasks are tracked
        public void StartTracking(Guid taskId, string taskName)
        {
            if (_taskProgress.Count >= MaxTasks)
                RemoveOldestTask();

            _taskProgress.GetOrAdd(taskId, new TaskProgressResponse(taskId)
            {
                TaskName = taskName,
                StartTime = DateTime.UtcNow,
                PercentageComplete = 0,
                IsCompleted = false,
                IsSuccessful = false
            });
        }

        /// <summary>
        /// Report task progress and append status messages
        /// </summary>
        public void ReportProgress(Guid taskId, double percentage, string statusMessage = null)
        {
            if (_taskProgress.TryGetValue(taskId, out var progress))
            {
                progress.PercentageComplete = percentage;

                // Append status messages to the list
                if (!string.IsNullOrEmpty(statusMessage))
                {
                    lock (progress.StatusMessages)  // Ensure thread-safe updates to the list
                    {
                        progress.StatusMessages.Add(statusMessage);
                    }
                }

                if (percentage >= 100)
                {
                    progress.IsCompleted = true;
                    progress.ActualCompletionTime = DateTime.UtcNow;
                    progress.IsSuccessful = true;
                    lock (progress.StatusMessages)
                    {
                        progress.StatusMessages.Add("Task Completed Successfully");
                    }
                }
            }
        }

        /// <summary>
        /// Report errors for a specific task
        /// </summary>
        public void ReportError(Guid taskId, string errorMessage)
        {
            if (_taskProgress.TryGetValue(taskId, out var progress))
            {
                lock (progress.Errors)  // Ensure thread-safe modification of the Errors list
                {
                    progress.Errors.Add(errorMessage);
                }

                lock (progress.StatusMessages)
                {
                    progress.StatusMessages.Add($"Error Occurred: {errorMessage}");
                }

                progress.IsCompleted = false;
                progress.IsSuccessful = false;
                progress.ActualCompletionTime = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Retrieve the progress of a task
        /// </summary>
        public TaskProgressResponse GetProgress(Guid taskId)
        {
            if (_taskProgress.TryGetValue(taskId, out var progress))
                return progress;

            return null; // Task not found
        }

        /// <summary>
        /// Remove the oldest task based on StartTime
        /// </summary>
        private void RemoveOldestTask()
        {
            var oldestTask = _taskProgress.OrderBy(kv => kv.Value.StartTime).FirstOrDefault();

            if (oldestTask.Key != Guid.Empty)
                _taskProgress.TryRemove(oldestTask.Key, out _);
        }
    }

}
