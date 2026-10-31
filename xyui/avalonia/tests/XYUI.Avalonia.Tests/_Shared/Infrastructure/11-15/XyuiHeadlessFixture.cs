using Avalonia.Headless;

namespace XYUI.Avalonia.Tests;

// Headless contract session: UI tests run on an isolated UI thread.
// This fixture does not prove desktop usability or visual appearance.
public sealed class XyuiHeadlessFixture : IAsyncLifetime
{
    readonly HeadlessUnitTestSession _session =
        HeadlessUnitTestSession.StartNew(typeof(XyuiTestAppBuilder));

    public T Run<T>(Func<T> action) =>
        _session.Dispatch(action, CancellationToken.None).GetAwaiter().GetResult();

    public void Run(Action action) =>
        _session.Dispatch(action, CancellationToken.None).GetAwaiter().GetResult();

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _session.DisposeAsync().AsTask();
}
