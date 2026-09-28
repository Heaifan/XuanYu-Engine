using Avalonia.Headless;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class UiHeadlessFixture : IAsyncLifetime
{
    readonly HeadlessUnitTestSession _session;

    public UiHeadlessFixture()
    {
        _session = HeadlessUnitTestSession.StartNew(typeof(UiTestAppBuilder));
    }

    public T Run<T>(Func<T> action) =>
        _session.Dispatch(() =>
        {
            UiTestPlatformServices.Install();
            return action();
        }, CancellationToken.None).GetAwaiter().GetResult();

    public void Run(Action action) =>
        _session.Dispatch(() =>
        {
            UiTestPlatformServices.Install();
            action();
        }, CancellationToken.None).GetAwaiter().GetResult();

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _session.DisposeAsync().AsTask();
}
