using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.About;
using XuanYu.Engine.Editor.Windows.Preferences;

namespace XuanYu.Engine.Editor.Windows.Shell.Windows;

/// <summary>编辑器窗口命令路由。管理 Preferences / InputBindings / About 窗口的打开和 Activate。</summary>
public sealed class EditorShellWindowRoute
{
    private Window? _preferencesWindow;
    private Window? _aboutWindow;

    public EditorShellWindowResult Open(EditorShellWindowCommand command)
    {
        return command switch
        {
            EditorShellWindowCommand.Preferences => OpenPreferences("已打开偏好设置。"),
            EditorShellWindowCommand.InputBindings => OpenPreferences("已打开键位设置。"),
            EditorShellWindowCommand.About => OpenAbout(),
            _ => new EditorShellWindowResult(null)
        };
    }

    private EditorShellWindowResult OpenPreferences(string logMessage)
    {
        if (_preferencesWindow is { IsVisible: true }) { _preferencesWindow.Activate(); return new(null); }
        _preferencesWindow = new EditorPreferencesWindow();
        var weak = _preferencesWindow;
        _preferencesWindow.Closed += (_, _) => { if (weak == _preferencesWindow) _preferencesWindow = null; };
        _preferencesWindow.Show();
        return new(logMessage);
    }

    private EditorShellWindowResult OpenAbout()
    {
        if (_aboutWindow is { IsVisible: true }) { _aboutWindow.Activate(); return new(null); }
        _aboutWindow = new AboutFluidWarfareWindow();
        var weak = _aboutWindow;
        _aboutWindow.Closed += (_, _) => { if (weak == _aboutWindow) _aboutWindow = null; };
        _aboutWindow.Show();
        return new("已打开关于 FluidWarfare。");
    }
}
