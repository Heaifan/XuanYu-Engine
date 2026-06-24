using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Presentation;

/// <summary>MoveGizmoFrameSource.Build 的纯输入。所有数据在调用时冻结。</summary>
public readonly record struct MoveGizmoFrameInput(
    bool MoveToolActive,
    EntityId SelectedEntityId,
    Vector3d EntityPosition,
    PresentedCameraSnapshot Camera,
    MoveGizmoElement HoveredElement,
    int SelectionRevision);
