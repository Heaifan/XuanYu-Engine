using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Navigation;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.ViewportNavigation;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Input.Picking;

/// <summary>地面悬停路由。鼠标移动时执行 CPU 射线-地面求交，更新状态栏坐标。</summary>
public sealed class EditorGroundHoverInputRoute
{
    /// <summary>鼠标在视口内移动 → 调度合并（最多 ~16ms 一次）→ 射线求交 → 状态栏反馈。</summary>
    public void HandlePointerMoved(EditorGroundHoverInputRequest r, VulkanViewportNativeHostInfo host)
    {
        if (!host.HasNativeHandle || host.Width < 1 || host.Height < 1) return;
        var s = r.Lifecycle.State.Session;
        if (s is null || s.Status != VulkanScene3dSessionStatus.Active) return;

        var snap = s.LastPresentedSnapshot;
        if (!snap.IsValid) return;

        if (VulkanSceneRayBuilder.TryBuild(r.X, r.Y, snap, (uint)host.Width, (uint)host.Height, out var ray) != SceneRayBuildStatus.Success || ray is null)
        { r.SetGroundPosition("地面坐标：无"); r.GroundPointerState.SetHover(null, null); return; }

        var hit = SceneRayGroundIntersection.Intersect(ray, SceneGroundPlane.Default);
        if (hit.IsHit && hit.WorldPosition is not null)
        { r.GroundPointerState.SetHover(hit.WorldPosition.Value, "鼠标"); r.SetGroundPosition($"地面坐标：X {hit.WorldPosition.Value.X:F2} | Y {hit.WorldPosition.Value.Y:F2} | Z {hit.WorldPosition.Value.Z:F2}"); }
        else
        { r.GroundPointerState.SetHover(null, null); r.SetGroundPosition("地面坐标：无"); }
    }

    /// <summary>鼠标离开视口 → 清除坐标和导航悬停。返回导航悬停清除结果供 Shell ApplyOverlayVisualState。</summary>
    public ViewportNavigationMoveResponse HandlePointerLeft(EditorGroundHoverInputRequest r, EditorSelectionRoute selection)
    {
        var nav = r.NavigationRoute.DragMode == ViewportNavigationDragMode.None
            ? r.NavigationRoute.ClearHover() : new(false, false, false, ViewportNavigationElement.None, ViewportNavigationElement.None);
        r.GroundPointerState.SetHover(null, null);
        r.SetCurrentSelection(selection.State.SelectedWorldEntity is not null ? selection.State.SelectedWorldEntity.DisplayName : "无");
        r.SetGroundPosition("地面坐标：无");
        return nav;
    }
}
