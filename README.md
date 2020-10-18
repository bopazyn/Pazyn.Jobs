# Pazyn.Jobs

`Pazyn.Jobs` is library for running recurring jobs.

## Minimal working example

```
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
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddJobs()
            .AddJob<EveryMinuteSecondJob>("* * * * *");
    }

    public void Configure(IApplicationBuilder app)
    {
    }
}
```