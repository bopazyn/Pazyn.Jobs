using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pazyn.Jobs
{
    public static class BackgroundTasksExtensions
    {
        public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
        {
            services.TryAddSingleton<IBackgroundTasksQueue, BackgroundTasksQueue>();
            services.AddHostedService<BackgroundTasksHostedService>();
            return services;
        }
    }
}