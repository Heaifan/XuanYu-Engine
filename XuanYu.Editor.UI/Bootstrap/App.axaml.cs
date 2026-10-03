using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using XuanYu.Render.Abstractions;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Scene;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;
using XYUI.Avalonia.Vector;

namespace XuanYu.Editor.UI;

public sealed class App : Application
{
    readonly INativeHostSurfaceBridgeFactory? _surfaceBridgeFactory;
    readonly Func<Func<bool>, bool, (SceneStateOwner Scene, MapEditSession Map)>? _stateFactory;

    public App() { }

    public App(INativeHostSurfaceBridgeFactory surfaceBridgeFactory) =>
        _surfaceBridgeFactory = surfaceBridgeFactory;

    public App(INativeHostSurfaceBridgeFactory surfaceBridgeFactory,
        Func<Func<bool>, bool, (SceneStateOwner Scene, MapEditSession Map)> stateFactory)
        : this(surfaceBridgeFactory) => _stateFactory = stateFactory;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries());
        Resources.MergedDictionaries.Add(XyuiVectorIcons.CreateResources());
        Styles.Add(XyuiTextStyles.Create());
        Styles.Add(XyuiShapeStyles.Create());
        Styles.Add(XyuiInteractionStyles.Create());
        Styles.Add(XyuiControlStyles.Create());
        Styles.Add(XyuiComponentStyles.Create());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var window = new UiWin();
            var isWriteThread = () => Dispatcher.UIThread.CheckAccess();
            var state = _stateFactory?.Invoke(isWriteThread, false);
            var vm = state is { } dependencies
                ? new UiVm(_surfaceBridgeFactory, dependencies.Scene, dependencies.Map,
                    isWriteThread, window)
                : new UiVm(_surfaceBridgeFactory, isWriteThread,
                    seedInitialScene: false, dialogService: window);
            window.DataContext = vm;
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
