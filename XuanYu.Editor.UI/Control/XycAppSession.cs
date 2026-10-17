using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace XuanYu.Editor.UI.XycControl;

public static class XycAppSession
{
    public static bool TryStart(string[] args, UiVm vm,
        IClassicDesktopStyleApplicationLifetime lifetime) => TryStart(args, vm, lifetime, null);

    internal static bool TryStart(string[] args, UiVm vm,
        IClassicDesktopStyleApplicationLifetime lifetime,
        SilentVulkanHost? silentHost = null)
    {
        var sessionId = Value(args, "--xyc-session");
        if (sessionId is null || !args.Contains("--xyc-stdio")) return false;
        Console.SetOut(Console.Error);
        var terrainLoad = StartTerrainLoad(args, vm);
        var output = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        var processor = new XycCommandProcessor(vm.CreateXycRuntime(sessionId, terrainLoad, silentHost),
            args.Contains("--xyc-diagnostic-write"));
        var host = new XycStdioHost(processor);
        _ = Task.Run(() => host.RunAsync(Console.In, output,
            () => Dispatcher.UIThread.Post(() =>
            {
                try { silentHost?.Dispose(); }
                finally { lifetime.Shutdown(); }
            }), CancellationToken.None));
        return true;
    }

    static Task<bool>? StartTerrainLoad(string[] args, UiVm vm)
    {
        var path = Value(args, "--xyc-test-dem");
        if (path is null) return null;
        path = XycTestDemPolicy.Resolve(AppContext.BaseDirectory, path);
        var completion = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(async () =>
        {
            try { completion.TrySetResult(await vm.ImportTerrainSourceAsync(path)); }
            catch (Exception error) { completion.TrySetException(error); }
        });
        return completion.Task;
    }

    static string? Value(string[] args, string key)
    {
        var index = Array.IndexOf(args, key);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}
