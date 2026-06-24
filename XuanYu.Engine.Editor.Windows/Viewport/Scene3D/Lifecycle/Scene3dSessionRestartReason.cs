namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;

/// <summary>Scene3D 会话重启原因。</summary>
public enum Scene3dSessionRestartReason
{
    Manual,
    ResizeFallback,
    GateReevaluated,
}
