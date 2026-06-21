using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

namespace FluidWarfare.Editor.Windows.Shell.Input.Transform;

/// <summary>SceneTool 输入路由的结果。携带视口事件响应值。</summary>
public sealed record EditorSceneToolInputResult(
    ViewportSceneToolPressResult? PressResult,
    bool Released);
