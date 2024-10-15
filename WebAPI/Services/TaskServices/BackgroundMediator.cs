using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;
using MediatR;

namespace WebAPI.Services.TaskServices
{
    public interface IBackgroundMediator
    {
        // Method for sending commands without expecting a response (fire-and-forget)
        Task SendAsync(BackgroundCommand<Unit> command);

        // Method for sending commands and awaiting a response (request-reply)
        Task<TResponse> SendAsync<TResponse>(BackgroundCommand<TResponse> command);
    }

    public class BackgroundMediator : IBackgroundMediator
    {
        private readonly Channel<IBackgroundCommand> _commandChannel;

        public BackgroundMediator(Channel<IBackgroundCommand> commandChannel)
        {
            _commandChannel = commandChannel;
        }

        /// <summary>
        /// Fire-and-forget method (no response expected)
        /// </summary>
        public async Task SendAsync(BackgroundCommand<Unit> command)
        {
            await _commandChannel.Writer.WriteAsync(command);
        }

        /// <summary>
        /// Request-reply method (response expected)
        /// </summary>
        public async Task<TResponse> SendAsync<TResponse>(BackgroundCommand<TResponse> command)
        {
            await _commandChannel.Writer.WriteAsync(command);

            // Await the result using TaskCompletionSource (no polling required)
            return await command.Task;
        }
    }




}
