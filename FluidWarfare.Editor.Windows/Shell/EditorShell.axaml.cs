using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluidWarfare.Core.Logging;
using FluidWarfare.Editor.Windows.Panels.Logging;

namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    public EditorShell()
    {
        AvaloniaXamlLoader.Load(this);
        InitializeLogs();
    }

    private void InitializeLogs()
    {
        var logs = CreateStartupLogs()
            .Select(entry => entry.ToDisplayString())
            .ToArray();

        this.FindControl<LogPanel>("EditorLogPanel")?.SetLogMessages(logs);
    }

    private static IReadOnlyList<EngineLogEntry> CreateStartupLogs()
    {
        return
        [
            EngineLogEntry.Create(
                0.0,
                EngineLogLevel.Info,
                "Editor",
                "FluidWarfare Editor 启动完成。"),

            EngineLogEntry.Create(
                0.0,
                EngineLogLevel.Info,
                "Core",
                "Core 基础模块已加载。")
        ];
    }
}
