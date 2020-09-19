using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Pazyn.Jobs.AspNetCore
{
    public static class JobsExtensions
    {
        public static IEndpointConventionBuilder MapJobTrigger(this IEndpointRouteBuilder endpoints, String path, String identifier) =>
            endpoints.MapGet(path, ctx =>
            {
                var jobQueue = ctx.RequestServices.GetRequiredService<IJobQueue>();
                jobQueue.QueueJobFromStore(identifier);
                return Task.CompletedTask;
            });
    }
}
