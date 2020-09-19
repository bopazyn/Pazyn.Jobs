using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pazyn.Jobs
{
    internal class JobQueue : IJobQueue
    {
        private IOptions<JobsContext> Options { get; }
        private IBackgroundTasksQueue BackgroundTasksQueue { get; }

        public JobQueue(IOptions<JobsContext> options, IBackgroundTasksQueue backgroundTasksQueue)
        {
            Options = options;
            BackgroundTasksQueue = backgroundTasksQueue;
        }

        public void QueueJob<T>() where T : IJob
        {
            QueueJob(sp => ActivatorUtilities.CreateInstance<T>(sp));
        }

        public void QueueJob(Func<IJob> jobFactory)
        {
            QueueJob(sp => jobFactory());
        }

        public void QueueJob(Func<IServiceProvider, IJob> jobFactory) =>
            BackgroundTasksQueue.QueueBackgroundTask((sp, cancellationToken) =>
                jobFactory(sp).Run(cancellationToken));

        public void QueueJobFromStore(String identifier)
        {
            var jobItems = Options.Value.JobItems.Where(x => x.Identifier == identifier).ToArray();
            if (jobItems.Length == 0)
            {
                throw new InvalidOperationException($"There is no registered job with identifier \"{identifier}\"");
            }

            if (jobItems.Length > 1)
            {
                throw new InvalidOperationException($"There are multiple job registered with identifier \"{identifier}\"");
            }

            BackgroundTasksQueue.QueueBackgroundTask((sp, ct) => jobItems.First().Run(sp, ct));
        }
    }
}