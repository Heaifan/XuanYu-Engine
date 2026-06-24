using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Submit;

/// <summary>Scene3dFrameSubmitRoute.Request 的输入。所有数据在调用时冻结。</summary>
public readonly record struct Scene3dFrameSubmitInput(
    VulkanScene3dFrameReason Reason,
    SceneOrbitCameraState CameraState,
    int CameraRevision,
    int RenderSeq,
    bool MoveToolActive,
    EntityId SelectedEntityId,
    Vector3d EntityPosition,
    MoveGizmoElement HoveredElement,
    int SelectionRevision);
