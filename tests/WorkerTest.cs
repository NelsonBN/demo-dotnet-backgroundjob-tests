using Microsoft.Extensions.Logging;
using NSubstitute;

namespace tests;

public class WorkerTest
{
    [Fact]
    public async void Test()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DemoWorkerWithDelay>>();

        var worker = new DemoWorkerWithDelay(logger);

        var tcs = new TaskCompletionSource<bool>();

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(10);

        cancellationTokenSource.Token.Register(() => tcs.TrySetResult(true));


        // Act
        await worker.StartAsync(cancellationTokenSource.Token);
        await tcs.Task;


        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }
}
