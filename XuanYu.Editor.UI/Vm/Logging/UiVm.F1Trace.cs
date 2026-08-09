using System.Reflection;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void LogF1Trace(string stage, string message)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            $"[F1TRACE-{stage}] {message}", "F1 区域绘制运行时取证");
        RefreshLogBindings();
    }

    void LogBuildProvenance()
    {
        var app = Assembly.GetEntryAssembly() ?? typeof(UiVm).Assembly;
        if (!app.GetName().Name!.Equals("XuanYu.Editor.App", StringComparison.Ordinal)) return;
        LogF1Trace("VERSION", $"App ProductVersion={Product(app)}; UI ProductVersion={Product(typeof(UiVm).Assembly)}");
        LogF1Trace("VERSION", $"App DLL={app.Location}; UI DLL={typeof(UiVm).Assembly.Location}");
    }

    static string Product(Assembly assembly) =>
        assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
}
