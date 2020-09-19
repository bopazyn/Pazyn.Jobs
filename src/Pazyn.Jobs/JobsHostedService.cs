using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pazyn.Jobs
{
    internal class JobsHostedService : IHostedService, IDisposable
    {
        public JobsHostedService(
            ILogger<JobsHostedService> logger,
            IOptions<JobsContext> options,
            IJobQueue jobQueue)
        {
            Logger = logger;
            Options = options;
            JobQueue = jobQueue;
        }

        private DateTimeOffset LastRun { get; set; } = DateTimeOffset.Now;
        private Timer Timer { get; set; }
        private ILogger Logger { get; }
        private IOptions<JobsContext> Options { get; }
        private IJobQueue JobQueue { get; }

        public void Dispose()
        {
            Timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation(Resources.JobsHostedServiceStarted);
            Timer = new Timer(DoWork, null, TimeSpan.Zero, Options.Value.JobsOptions.Period);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation(Resources.JobsHostedServiceStopped);
            Timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoWork(Object state)
        {
            var jobsToRun = Options.Value.GetJobsToRun(LastRun, DateTimeOffset.Now);
            Logger.LogInformation(Resources.JobsHostedServiceEnqueuedJobs, jobsToRun.Length);
            foreach (var jobToRun in jobsToRun)
            {
                JobQueue.QueueJob(jobToRun.JobFactory);
            }

            LastRun = DateTimeOffset.Now;
        }
    }
}