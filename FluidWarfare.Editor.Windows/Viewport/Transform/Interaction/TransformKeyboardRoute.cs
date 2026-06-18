using FluidWarfare.Core.Identity;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Editor.Windows.Viewport.Transform.Drag;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;

/// <summary>
/// 键盘变换交互路由。只处理 G/Enter/Esc 三键。
/// FocusLost 不在此路由处理，Shell 直接调用 TransformPointerRoute.Cancel。
/// </summary>
public static class TransformKeyboardRoute
{
    public static TransformInteractionResult HandleKeyDown(
        int vkCode,
        TransformInteractionState state,
        TransformPointerRoute pointerRoute,
        EntityId? selectedEntity,
        TransformStartSnapshot? gModalSnapshot,
        int lastPointerX, int lastPointerY)
    {
        // Esc: 取消活动变换或 G 模态
        if (vkCode == 0x1B && (state.BlenderMoveActive || pointerRoute.IsDragActive))
        {
            state.SetBlenderGActive(false);
            pointerRoute.Cancel();
            return new TransformInteractionResult(
                TransformInteractionAction.Cancelled, default, TransformInteractionReason.Escape);
        }

        // G: 启动 Blender G 模态自由移动
        if (vkCode == 0x47 && selectedEntity is not null && gModalSnapshot is not null
            && !pointerRoute.IsDragActive)
        {
            if (!state.MoveToolActive) state.SetToolActive(true);

            var req = new TransformStartRequest(
                TransformStartSource.BlenderG, MoveGizmoElement.ViewPlane,
                lastPointerX, lastPointerY);
            var result = pointerRoute.OnPointerPressed(req, gModalSnapshot.Value);
            if (result.Action == TransformInteractionAction.Started)
                state.SetBlenderGActive(true);
            return result;
        }

        // Enter: G 模态确认
        if (vkCode == 0x0D && state.BlenderMoveActive)
        {
            state.SetBlenderGActive(false);
            return pointerRoute.OnPointerReleased();
        }

        return default;
    }
}
