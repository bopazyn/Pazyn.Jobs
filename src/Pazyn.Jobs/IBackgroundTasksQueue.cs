using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pazyn.Jobs
{
    public interface IBackgroundTasksQueue
    {
        void QueueBackgroundTask(Func<IServiceProvider, CancellationToken, Task> backgroundTask, TimeSpan? timeout = default);
        ValueTask<BackgroundTask> DequeueAsync(CancellationToken cancellationToken = default);
    }
}