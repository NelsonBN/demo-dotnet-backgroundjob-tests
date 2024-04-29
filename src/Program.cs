var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<DemoWorkerWithDelay>();
builder.Services.AddHostedService<TimedWorker>();

var host = builder.Build();
host.Run();




public class DemoWorkerWithDelay(ILogger<DemoWorkerWithDelay> logger) : BackgroundService
{
    private readonly ILogger<DemoWorkerWithDelay> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Task.Delay(1_000, stoppingToken);
        }
    }
}


// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
public class TimedWorker(ILogger<TimedWorker> logger, TimeProvider timeProvider) : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedWorker> _logger = logger;
    private readonly TimeProvider _timeProvider = timeProvider;
    private ITimer? _timer = null;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = _timeProvider.CreateTimer(
            DoWork,
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref executionCount);

        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}