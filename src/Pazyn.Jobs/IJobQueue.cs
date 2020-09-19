using System;

namespace Pazyn.Jobs
{
    public interface IJobQueue
    {
        void QueueJob<T>() where T : IJob;
        void QueueJob(Func<IJob> jobFactory);
        void QueueJob(Func<IServiceProvider, IJob> jobFactory);

        void QueueJobFromStore(String identifier);
    }
}