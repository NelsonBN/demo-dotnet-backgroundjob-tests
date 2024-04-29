using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace tests;

public class TimedWorkerTest
{
    [Fact]
    public async Task Test()
    {
        // Arrange
        var logger = Substitute.For<ILogger<TimedWorker>>();
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var worker = new TimedWorker(logger, timeProvider);


        // Act
        await worker.StartAsync(default);
        timeProvider.Advance(TimeSpan.FromSeconds(6));


        // Assert
        logger
            .Received(2)
            .Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
    }
}
