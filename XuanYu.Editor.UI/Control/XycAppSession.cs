using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace XuanYu.Editor.UI.XycControl;

public static class XycAppSession
{
    public static bool TryStart(string[] args, UiVm vm,
        IClassicDesktopStyleApplicationLifetime lifetime)
    {
        var sessionId = Value(args, "--xyc-session");
        if (sessionId is null || !args.Contains("--xyc-stdio")) return false;
        Console.SetOut(Console.Error);
        var output = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        var processor = new XycCommandProcessor(vm.CreateXycRuntime(sessionId),
            args.Contains("--xyc-diagnostic-write"));
        var host = new XycStdioHost(processor);
        _ = host.RunAsync(Console.In, output,
            () => Dispatcher.UIThread.Post(() => lifetime.Shutdown()), CancellationToken.None);
        return true;
    }

    static string? Value(string[] args, string key)
    {
        var index = Array.IndexOf(args, key);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}
