using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed class App : Application
{
    readonly INativeHostSurfaceBridgeFactory? _surfaceBridgeFactory;

    public App() { }

    public App(INativeHostSurfaceBridgeFactory surfaceBridgeFactory) =>
        _surfaceBridgeFactory = surfaceBridgeFactory;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new UiWin();
            var vm = new UiVm(_surfaceBridgeFactory, seedInitialScene: false, dialogService: window);
            window.DataContext = vm;
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
