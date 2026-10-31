using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Scene;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

internal static class XycSilentApplicationStartup
{
    public static void Start(IClassicDesktopStyleApplicationLifetime desktop,
        INativeHostSurfaceBridgeFactory? bridgeFactory,
        Func<Func<bool>, bool, (SceneStateOwner Scene, MapEditSession Map)>? stateFactory)
    {
        Console.SetOut(Console.Error);
        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        var isWriteThread = () => Dispatcher.UIThread.CheckAccess();
        var state = stateFactory?.Invoke(isWriteThread, false);
        var vm = state is { } dependencies
            ? new UiVm(bridgeFactory, dependencies.Scene, dependencies.Map, isWriteThread)
            : new UiVm(bridgeFactory, isWriteThread, seedInitialScene: false);
        SilentVulkanHost host;
        try { host = SilentVulkanHost.Start(vm, bridgeFactory); }
        catch (Exception error)
        {
            Fail(desktop, 70, $"静默 Vulkan 宿主启动失败：{error}");
            return;
        }
        if (XycAppSession.TryStart(desktop.Args ?? [], vm, desktop, host)) return;
        host.Dispose();
        Fail(desktop, 64, "静默模式需要 --xyc-stdio 与 --xyc-session。");
    }

    static void Fail(IClassicDesktopStyleApplicationLifetime desktop, int code, string message)
    {
        Console.Error.WriteLine($"[XYC-SILENT-START-FAIL] {message}");
        Environment.ExitCode = code;
        desktop.Shutdown();
    }
}
