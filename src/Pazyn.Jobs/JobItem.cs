using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Pazyn.Jobs
{
    public class JobItem
    {
        public Func<IServiceProvider, CancellationToken, Task> Run => (sp, ct) => JobFactory(sp).Run(ct);
        public Func<IServiceProvider, IJob> JobFactory { get; private set; }
        public CronExpression CronExpression { get; }
        public String Identifier { get; set; }

        public JobItem(Func<IServiceProvider, IJob> jobFactory, CronExpression cronExpression)
        {
            JobFactory = jobFactory;
            CronExpression = cronExpression;
        }

        public void Decorate<TDec>() where TDec : IJob =>
            Decorate((sp, job) => ActivatorUtilities.CreateInstance<TDec>(sp, job));

        public void Decorate(Func<IJob, IJob> taskFactory) =>
            Decorate((sp, job) => taskFactory(job));

        public void Decorate(Func<IServiceProvider, IJob, IJob> jobFactory)
        {
            var tmp = JobFactory;
            JobFactory = sp => jobFactory(sp, tmp(sp));
        }
    }
}