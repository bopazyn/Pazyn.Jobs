using System;
using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Pazyn.Jobs
{
    internal class JobsBuilder : IJobsBuilder
    {
        private IServiceCollection Services { get; }

        public JobsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IJobsBuilder AddJob<TJob>(String cronExpression, Action<JobItem> configureItem = null) where TJob : IJob =>
            AddJob(cronExpression, sp => ActivatorUtilities.CreateInstance<TJob>(sp), configureItem);

        public IJobsBuilder AddJob(String cronExpression, Func<IJob> jobFactory, Action<JobItem> configureItem = null) =>
            AddJob(cronExpression, sp => jobFactory(), configureItem);

        public IJobsBuilder AddJob(String cronExpression, Func<IServiceProvider, IJob> jobFactory, Action<JobItem> configureItem = null)
        {
            var jobStorageItem = new JobItem(jobFactory, CronExpression.Parse(cronExpression));
            configureItem?.Invoke(jobStorageItem);

            Services.Configure<JobsContext>(options => options.JobItems.Add(jobStorageItem));

            return this;
        }
    }
}