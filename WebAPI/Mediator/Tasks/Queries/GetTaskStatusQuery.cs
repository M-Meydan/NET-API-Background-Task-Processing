using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Services.TaskServices;

namespace WebAPI.Mediator.Tasks.Queries
{
    public class GetTaskProgressQuery : IRequest<TaskProgressResponse>
    {
        public Guid Id { get; set; }

        public GetTaskProgressQuery() { }
        public GetTaskProgressQuery(Guid id)
        {
            Id = id;
        }

        public class GetTaskStatusQueryHandler : IRequestHandler<GetTaskProgressQuery, TaskProgressResponse>
        {
            private readonly ITaskProgressService _taskProgressService;

            public GetTaskStatusQueryHandler(ITaskProgressService taskProgressService)
            {
                _taskProgressService = taskProgressService;
            }

            public async Task<TaskProgressResponse> Handle(GetTaskProgressQuery query, CancellationToken cancellationToken)
            {
                // Fetch the task progress using the provided task ID
                var progress = _taskProgressService.GetProgress(query.Id);

                // Return the progress (or a new empty response if no progress found)
                if (progress == null)
                {

                    progress = new TaskProgressResponse(query.Id);
                    progress.Errors.Add("Task Not Found");
                }

                return progress;
            }
        }
    }

}
