using Avalonia.Headless;

namespace XYUI.Avalonia.Tests;

// Headless 会话（玄域先例模式）：所有 UI 测试在独立 UI 线程内执行
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
