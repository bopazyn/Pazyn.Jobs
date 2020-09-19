using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Pazyn.Jobs
{
    internal class BackgroundTasksQueue : IBackgroundTasksQueue
    {
        private Channel<BackgroundTask> Queue { get; } = Channel.CreateUnbounded<BackgroundTask>(new UnboundedChannelOptions
        {
            SingleReader = true,
        });

        public void QueueBackgroundTask(Func<IServiceProvider, CancellationToken, Task> backgroundTask, TimeSpan? timeout = default)
        {
            if (backgroundTask == null)
            {
                throw new ArgumentNullException(nameof(backgroundTask));
            }

            var cancellationTokenSource = new CancellationTokenSource();
            if (timeout.HasValue)
            {
                cancellationTokenSource.CancelAfter(timeout.Value);
            }

            var backgroundTaskItem = new BackgroundTask(backgroundTask, cancellationTokenSource.Token);
            Queue.Writer.TryWrite(backgroundTaskItem);
        }

        public ValueTask<BackgroundTask> DequeueAsync(CancellationToken cancellationToken = default) =>
            Queue.Reader.ReadAsync(cancellationToken);
    }
}