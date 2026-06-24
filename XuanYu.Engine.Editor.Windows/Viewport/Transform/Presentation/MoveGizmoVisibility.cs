using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;

/// <summary>Move Gizmo 可见性判断。选中实体 + 相机有效即显示。</summary>
public static class MoveGizmoVisibility
{
    public static bool ShouldShow(EntityId? selectedEntity, PresentedCameraSnapshot camera) =>
        selectedEntity is not null && camera.IsValid;
}
