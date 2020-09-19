using System;

namespace Pazyn.Jobs
{
    public interface IJobsBuilder
    {
        IJobsBuilder AddJob<TJob>(String cronExpression, Action<JobItem> configureItem = null) where TJob : IJob;
        IJobsBuilder AddJob(String cronExpression, Func<IJob> jobFactory, Action<JobItem> configureItem = null);
        IJobsBuilder AddJob(String cronExpression, Func<IServiceProvider, IJob> jobFactory, Action<JobItem> configureItem = null);
    }
}