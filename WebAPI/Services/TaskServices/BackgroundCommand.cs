using MediatR;
using System;
using System.Threading.Tasks;

namespace WebAPI.Services.TaskServices
{
    public interface IBackgroundCommand
    {
        Guid Id { get; set; }
        Task Completion { get; }          
        void SetResult(object result);  
        void SetException(Exception ex);  
    }


    public class BackgroundCommand<TResponse> : IRequest<TResponse>, IBackgroundCommand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        private readonly TaskCompletionSource<TResponse> _taskCompletionSource = new();

        /// <summary>
        /// The non-generic Task to represent completion for IBackgroundCommand
        /// </summary>
        public Task Completion => _taskCompletionSource.Task;

        /// <summary>
        /// The generic task for retrieving the result
        /// </summary>
        public Task<TResponse> Task => _taskCompletionSource.Task;

        public void SetResult(object result)
        {
            if (result is TResponse typedResult)
                _taskCompletionSource.SetResult(typedResult); // Set the result if the type matches
            else
                _taskCompletionSource.SetException(new InvalidCastException("Result type mismatch"));
        }

        public void SetException(Exception ex) =>   _taskCompletionSource.SetException(ex);

    }
}
