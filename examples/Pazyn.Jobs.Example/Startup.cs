using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pazyn.Jobs.AspNetCore;

namespace Pazyn.Jobs.Example
{
    public class RepeatJobDecorator : IJob
    {
        private IJob InnerJob { get; }
        public Int32 Repeats { get; }

        public RepeatJobDecorator(IJob innerJob, Int32 repeats)
        {
            InnerJob = innerJob;
            Repeats = repeats;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            for (var i = 0; i < Repeats; i++)
            {
                await InnerJob.Run(cancellationToken);
            }
        }
    }

    public class EveryMinuteSecondJob : IJob
    {
        private ILogger<EveryMinuteSecondJob> Logger { get; }

        public EveryMinuteSecondJob(ILogger<EveryMinuteSecondJob> logger) => Logger = logger;

        public Task Run(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"I am EveryMinuteSecondJob Time is {DateTimeOffset.Now}");
            return Task.CompletedTask;
        }
    }

    public class Startup
    {
        private const String JobId = "JobId";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJobs()
                .AddJob<EveryMinuteSecondJob>("* * * * *", ji =>
                {
                    ji.Identifier = JobId;
                    ji.Decorate(job => new RepeatJobDecorator(job, 5));
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context => context.Response.WriteAsync("Hello World!"));
                endpoints.MapJobTrigger("/run", JobId);
            });
        }
    }
}