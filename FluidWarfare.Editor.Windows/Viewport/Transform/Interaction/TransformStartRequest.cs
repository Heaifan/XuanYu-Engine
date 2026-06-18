using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;

/// <summary>变换拖动启动来源。严格区分不允许 G 模态伪造 Hover。</summary>
public enum TransformStartSource
{
    GizmoHandle,
    EntityBody,
    BlenderG,
}

/// <summary>拖动启动请求。由 Shell 构建，PointerRoute 消费。</summary>
public readonly record struct TransformStartRequest(
    TransformStartSource Source,
    MoveGizmoElement GizmoElement,
    double PointerX,
    double PointerY);
