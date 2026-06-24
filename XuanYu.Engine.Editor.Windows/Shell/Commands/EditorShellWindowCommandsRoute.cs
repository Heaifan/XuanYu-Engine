using Avalonia.Interactivity;
using XuanYu.Engine.Editor.Windows.Shell.Windows;

namespace XuanYu.Engine.Editor.Windows.Shell.Commands;

/// <summary>窗口菜单命令路由。负责 Preferences/About/InputBindings 菜单命令转发。</summary>
sealed class EditorShellWindowCommandsRoute(
    EditorShellWindowRoute windowRoute,
    Action<string> appendInfoLog)
{
    public void HandlePreferencesClicked(object? sender, RoutedEventArgs e) =>
        ApplyResult(windowRoute.Open(EditorShellWindowCommand.Preferences));

    public void HandleShowInputBindingsClicked(object? sender, RoutedEventArgs e) =>
        ApplyResult(windowRoute.Open(EditorShellWindowCommand.InputBindings));

    public void HandleAboutFluidWarfareClicked(object? sender, RoutedEventArgs e) =>
        ApplyResult(windowRoute.Open(EditorShellWindowCommand.About));

    public void ExecuteOpenPreferences()
    {
        var r = windowRoute.Open(EditorShellWindowCommand.Preferences);
        if (r.LogMessage is not null) appendInfoLog(r.LogMessage);
    }

    void ApplyResult(EditorShellWindowResult r)
    {
        if (r.LogMessage is not null) appendInfoLog(r.LogMessage);
    }
}
