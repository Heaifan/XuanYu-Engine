using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class UiRuntimeTestHost : IDisposable
{
    readonly UiHeadlessFixture _fixture;
    Window? _window;

    public UiRuntimeTestHost(UiHeadlessFixture fixture) => _fixture = fixture;

    public T Run<T>(Func<T> action) => _fixture.Run(action);

    public void Run(Action action) => _fixture.Run(action);

    public Window Show(Control content, double width = 300, double height = 420)
    {
        var window = new Window { Width = width, Height = height, Content = content };
        window.Show();
        window.UpdateLayout();
        _window = window;
        return window;
    }

    public static IEnumerable<T> Descendants<T>(Visual root) where T : Visual =>
        root.GetVisualDescendants().OfType<T>();

    public void Dispose()
    {
        if (_window is not null) Run(() => _window.Close());
    }
}
