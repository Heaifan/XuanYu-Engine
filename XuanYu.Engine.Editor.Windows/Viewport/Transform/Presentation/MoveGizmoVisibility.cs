using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;

/// <summary>Move Gizmo 可见性判断。纯计算，不访问 Vulkan/Shell/WorldState。</summary>
public static class MoveGizmoVisibility
{
    public static bool ShouldShow(bool moveToolActive, EntityId? selectedEntity,
        PresentedCameraSnapshot camera) =>
        moveToolActive && selectedEntity is not null && camera.IsValid;
}
