using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;

namespace FluidWarfare.Editor.Windows.Shell.Composition;

/// <summary>Shell 组合根的结果。Shell 构造函数中拆包赋值给对应字段。</summary>
public sealed record EditorShellCompositionResult(
    EditorShellControlRefs Controls,
    EditorShellRouteSet Routes,
    Scene3dSessionLifecycle Lifecycle);
