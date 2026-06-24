using FluidWarfare.Engine.World;
using FluidWarfare.Editor.Windows.Shell;

namespace FluidWarfare.Editor.Windows.Viewport.Selection.Focus;

/// <summary>视口焦点处理的完整输出。Shell 无需分支直接 Apply。</summary>
public sealed record ViewportFocusSelectionResult(
    EditorSelection InspectorSelection,
    string StatusBarText,
    bool ShowEmptyWorld,
    IReadOnlyList<string> LogMessages,
    IReadOnlyList<string> LogWarnings,
    WorldEntityInfo? EntityToShow)
{
    public static readonly ViewportFocusSelectionResult EmptyWorld = new(
        new EditorSelection("编辑器占位区", "3D 视口", "当前 World 没有可显示实体。"),
        "3D 视口", true,
        ["视口获得焦点。"], ["当前 World 没有可显示实体。"], null);

    public static readonly ViewportFocusSelectionResult NoWorld = new(
        new EditorSelection("编辑器占位区", "3D 视口", "这里将显示 Vulkan 渲染的 3D 战场。"),
        "3D 视口", false,
        ["视口获得焦点。"], [], null);
}
