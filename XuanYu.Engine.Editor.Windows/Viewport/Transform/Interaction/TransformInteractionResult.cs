using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;

/// <summary>变换交互动作。日志层根据 Action + Reason 生成中文消息。</summary>
public enum TransformInteractionAction
{
    NotHandled,
    Started,
    Previewed,
    Confirmed,
    Cancelled,
}

/// <summary>变换交互原因。用于日志分类，不携带 UI 文案。</summary>
public enum TransformInteractionReason
{
    None,
    GizmoAxis,
    GizmoPlane,
    EntityBody,
    BlenderG,
    Escape,
    FocusLost,
}

/// <summary>变换交互的输出结果。使用 SceneTransform 确保旋转和缩放可复用。</summary>
public readonly record struct TransformInteractionResult(
    TransformInteractionAction Action,
    SceneTransform Transform,
    TransformInteractionReason Reason);
