using Avalonia.Controls;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Menu;

/// <summary>运行菜单路由。管理 RunMenuButton Flyout 和 Scene3D 菜单项状态。</summary>
public sealed class EditorRunMenuRoute
{
    private MenuItem? _runScene3dItem;

    /// <summary>请求重新启动 Scene3D 会话。Shell 订阅此事件执行实际重启。</summary>
    public event Action? RestartScene3dRequested;

    /// <summary>将路由绑定到 RunMenuButton。设置 Flyout 和菜单项。</summary>
    public void Attach(Button runMenuButton)
    {
        var flyout = new MenuFlyout();
        _runScene3dItem = new MenuItem { Header = "重新启动 Scene3D 会话" };
        _runScene3dItem.Click += (_, _) => RestartScene3dRequested?.Invoke();
        flyout.Items.Add(_runScene3dItem);
        runMenuButton.Flyout = flyout;
    }

    /// <summary>设置 Scene3D 菜单项是否可用。由诊断刷新时调用。</summary>
    public void SetScene3dEnabled(bool canRun)
    {
        if (_runScene3dItem is not null)
            _runScene3dItem.IsEnabled = canRun;
    }
}
