using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Project.World.Transform;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Drag;

/// <summary>
/// 拖动开始时的冻结快照。保存所有外部状态，DragRoute 不持有 Shell 引用。
/// 使用 SceneTransform 而非 Vector3d，确保旋转/缩放可复用此结构。
/// </summary>
public readonly record struct TransformStartSnapshot(
    EntityId EntityId,
    SceneTransform InitialTransform,
    bool IsDirty,
    PresentedCameraSnapshot Camera,
    PresentedMoveGizmoSnapshot Gizmo);
