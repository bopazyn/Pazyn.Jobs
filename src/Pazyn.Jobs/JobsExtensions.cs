using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pazyn.Jobs
{
    public static class JobsExtensions
    {
        public static IJobsBuilder AddJobs(this IServiceCollection services) =>
            services.AddJobs(x => { });

        public static IJobsBuilder AddJobs(this IServiceCollection services, Action<JobsOptions> configureOptions)
        {
            services.AddBackgroundTasks();

            services.Configure<JobsContext>(options =>
            {
                var jobsSupervisorOptions = new JobsOptions();
                configureOptions?.Invoke(jobsSupervisorOptions);
                options.JobsOptions = jobsSupervisorOptions;
            });

            services.TryAddSingleton<IJobQueue, JobQueue>();
            services.AddHostedService<JobsHostedService>();
            return new JobsBuilder(services);
        }
    }
}