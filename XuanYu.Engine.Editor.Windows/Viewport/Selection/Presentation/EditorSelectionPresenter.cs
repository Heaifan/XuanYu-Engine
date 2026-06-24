using FluidWarfare.Editor.Windows.Shell;
using XuanYu.Engine.World;

namespace FluidWarfare.Editor.Windows.Viewport.Selection.Presentation;

/// <summary>基础选择展示工具。默认选择等工厂方法。</summary>
public static class EditorSelectionPresenter
{
    public static EditorSelection CreateDefaultViewportSelection() => new(
        "编辑器占位区", "3D 视口", "这里将显示 Vulkan 渲染的 3D 战场。");
}
