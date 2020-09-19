using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pazyn.Jobs
{
    internal class BackgroundTasksHostedService : BackgroundService
    {
        public BackgroundTasksHostedService(
            IBackgroundTasksQueue tasksQueue,
            ILogger<BackgroundTasksHostedService> logger,
            IServiceProvider serviceProvider)
        {
            TasksQueue = tasksQueue;
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        private ILogger Logger { get; }
        private IBackgroundTasksQueue TasksQueue { get; }
        private IServiceProvider ServiceProvider { get; }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation(Resources.BackgroundTasksHostedServiceStarted);

            while (!cancellationToken.IsCancellationRequested)
            {
                var backgroundTaskItem = await TasksQueue.DequeueAsync(cancellationToken);
                try
                {
                    using var serviceScope = ServiceProvider.CreateScope();
                    var newCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, backgroundTaskItem.CancellationToken);
                    await backgroundTaskItem.Delegate(serviceScope.ServiceProvider, newCancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, Resources.BackgroundTaskException);
                }
            }

            Logger.LogInformation(Resources.BackgroundTasksHostedServiceStopped);
        }
    }
}