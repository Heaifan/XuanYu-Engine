using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Runtime;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;
using FluidWarfare.Editor.Windows.Panels.Viewport.Tools;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Picking;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;
using FluidWarfare.Editor.Windows.Viewport.Transform.Presentation;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Vulkan.Scene3D.Session;
using static FluidWarfare.Editor.Windows.Viewport.Transform.Interaction.TransformInteractionAction;
namespace FluidWarfare.Editor.Windows.Shell.Input;
public sealed class EditorViewportInputRoute
{
    public EditorViewportInputState State { get; } = new();
    public EditorViewportInputResult HandleKeyDown(EditorViewportInputRequest r)
    {
        var sel = r.SelectionRoute.State.SelectedWorldEntity;
        var snap = (r.KeyCode is 0x47 or 0x1B) && sel is not null ? r.BuildTransformSnapshot() : null;
        var kr = TransformKeyboardRoute.HandleKeyDown(r.KeyCode, r.PointerRoute, sel?.EntityId, snap, r.State.LastPointerX, r.State.LastPointerY);
        if (kr.Action == Started) { r.ToolPalette?.SetActiveTool(ViewportEditorTool.Move); r.InfoLog("G 移动：移动鼠标拖动，左键/Enter 确认，右键/Esc 取消"); return new(true); }
        if (kr.Action == Confirmed) { r.ApplyTransform(kr.Transform, EditorEntityTransformOrigin.MoveTool); r.InfoLog($"移动完成 ({kr.Transform.Position.X:F3}, {kr.Transform.Position.Y:F3}, {kr.Transform.Position.Z:F3})"); return new(true); }
        if (kr.Action == Cancelled) { r.CancelTransform(kr); r.InfoLog("变换已取消"); return new(true); }
        if (kr.Action != NotHandled) return new(true);
        return Trans(r, () => r.State.Translator?.OnRawKeyDown(r.KeyCode, r.State.LastPointerX, r.State.LastPointerY));
    }
    public EditorViewportInputResult HandleKeyUp(EditorViewportInputRequest r) { r.State.Translator?.OnRawKeyUp(r.KeyCode); return new(false); }
    public EditorViewportInputResult HandlePointerDown(EditorViewportInputRequest r)
    {
        r.State.LastPointerX = r.X; r.State.LastPointerY = r.Y;
        if (r.PointerRoute.IsBlenderGActive) { r.PointerRoute.SetBlenderGActive(false); if (r.ButtonCode == 1) return new(true); if (r.ButtonCode == 2) { var c = r.PointerRoute.Cancel(TransformInteractionReason.Escape); r.CancelTransform(c); r.InfoLog("移动已取消"); } return new(true); }
        return Trans(r, () => r.State.Translator?.OnRawPointerButtonDown(r.ButtonCode, r.X, r.Y));
    }
    public EditorViewportInputResult HandlePointerMoved(EditorViewportInputRequest r)
    {
        r.State.LastPointerX = r.X; r.State.LastPointerY = r.Y;
        if (r.PointerRoute.IsMoveToolActive) { var g = r.Lifecycle.State.FrameRoute?.Snapshots.PresentedGizmo; if (g?.IsAvailable == true) r.PointerRoute.UpdateGizmoHover(r.X, r.Y, g.Value.Layout); }
        if (r.PointerRoute.IsDragActive) { var dr = r.PointerRoute.OnPointerMoved(r.X, r.Y); if (dr.Action == Previewed) { r.ApplyPreviewPosition(); return new(true); } }
        return Trans(r, () => r.State.Translator?.OnRawPointerMoved(r.X, r.Y));
    }
    public EditorViewportInputResult HandlePointerUp(EditorViewportInputRequest r) { r.State.Translator?.OnRawPointerButtonUp(r.ButtonCode); return new(false); }
    public EditorViewportInputResult HandleMouseWheel(EditorViewportInputRequest r) { return Trans(r, () => r.State.Translator?.OnRawMouseWheel(r.WheelDelta, 0, r.State.LastPointerX, r.State.LastPointerY)); }
    public EditorViewportInputResult HandleFocusLost(EditorViewportInputRequest r)
    {
        r.State.Translator?.OnRawInputFocusLost();
        if (r.PointerRoute.IsDragActive || r.PointerRoute.IsBlenderGActive) { r.PointerRoute.SetBlenderGActive(false); r.CancelTransform(r.PointerRoute.Cancel(TransformInteractionReason.FocusLost)); }
        return new(true);
    }
    public ViewportSceneToolPressResult HandleSceneToolPressed(EditorViewportInputRequest r)
    {
        if (!r.PointerRoute.IsMoveToolActive || r.SelectionRoute.State.SelectedWorldEntity is null) return ViewportSceneToolPressResult.NotHandled;
        var cam = r.Lifecycle.State.Session?.LastPresentedSnapshot;
        if (cam is not { IsValid: true }) return ViewportSceneToolPressResult.NotHandled;
        var snap = r.BuildTransformSnapshot(); if (snap is null) return ViewportSceneToolPressResult.NotHandled;
        if (r.PointerRoute.HasHoveredElement) return r.PointerRoute.OnPointerPressed(new(TransformStartSource.GizmoHandle, MoveGizmoElement.ViewPlane, r.X, r.Y), snap.Value).Action == Started ? ViewportSceneToolPressResult.BeginDrag : ViewportSceneToolPressResult.NotHandled;
        var pick = r.PickRoute.Pick(new(r.X, r.Y, cam, r.Lifecycle.State.FrameRoute?.Snapshots.PresentedPick ?? PresentedScenePickSnapshot.None, r.RenderSceneStore.Current, SceneGroundPlane.Default));
        if (pick.Kind == ViewportPickKind.Entity && pick.EntityId == r.SelectionRoute.State.SelectedWorldEntity.EntityId) { r.InfoLog("实体本体拖动"); return r.PointerRoute.OnPointerPressed(new(TransformStartSource.EntityBody, MoveGizmoElement.ViewPlane, r.X, r.Y), snap.Value).Action == Started ? ViewportSceneToolPressResult.BeginDrag : ViewportSceneToolPressResult.NotHandled; }
        return ViewportSceneToolPressResult.NotHandled;
    }
    public TransformInteractionResult HandleSceneToolReleased(EditorViewportInputRequest r)
    {
        var result = r.PointerRoute.OnPointerReleased();
        if (result.Action == Confirmed) { r.ApplyTransform(result.Transform, EditorEntityTransformOrigin.MoveTool); r.InfoLog($"移动完成 ({result.Transform.Position.X:F3}, {result.Transform.Position.Y:F3}, {result.Transform.Position.Z:F3})"); }
        return result;
    }
    EditorViewportInputResult Trans(EditorViewportInputRequest r, Func<EditorInputMatch?> t)
    { if (r.State.Translator is null) return new(false); var m = t(); if (m?.IsMatch != true || m.Definition is null) return new(false); Exec(r, m); return new(true); }
    void Exec(EditorViewportInputRequest r, EditorInputMatch m)
    {
        var s = r.Lifecycle.State.Session; bool a() => s?.Status == VulkanScene3dSessionStatus.Active;
        switch (m.ActionId)
        {
            case "viewport.orbit": if (a()) { var c = r.CameraRoute.Apply(new ViewportCameraCommand.Orbit(-m.DeltaX, -m.DeltaY)); if (c.NeedsFrame) r.ScheduleFrame(c.Reason); } break;
            case "viewport.pan": if (a()) { var c = r.CameraRoute.Apply(new ViewportCameraCommand.Pan(m.DeltaX, m.DeltaY, 1)); if (c.NeedsFrame) r.ScheduleFrame(c.Reason); } break;
            case "viewport.dolly": if (a()) { var c = r.CameraRoute.Apply(new ViewportCameraCommand.Dolly(m.DeltaY)); if (c.NeedsFrame) r.ScheduleFrame(c.Reason); } break;
            case "viewport.zoom": if (a()) { var c = r.CameraRoute.Apply(new ViewportCameraCommand.Zoom(m.WheelDelta)); if (c.NeedsFrame) r.ScheduleFrame(c.Reason); } break;
            case "viewport.frame_all": if (s is not null) { var c = r.CameraRoute.Apply(new ViewportCameraCommand.FrameAll()); if (c.NeedsFrame) r.ScheduleFrame(c.Reason); } break;
            case "viewport.frame_selected": r.ExecuteFrameSelected(); break;
            case "viewport.toggle_projection": r.CameraRoute.Apply(new ViewportCameraCommand.ToggleProjection()); break;
            case "viewport.view_front" or "viewport.view_back" or "viewport.view_right" or "viewport.view_left" or "viewport.view_top" or "viewport.view_bottom":
                if (s?.Status == VulkanScene3dSessionStatus.Active) r.CameraRoute.Apply(new ViewportCameraCommand.SnapToView(m.ActionId switch { "viewport.view_front" => SceneNavigationView.PositiveY, "viewport.view_back" => SceneNavigationView.NegativeY, "viewport.view_right" => SceneNavigationView.PositiveX, "viewport.view_left" => SceneNavigationView.NegativeX, "viewport.view_top" => SceneNavigationView.PositiveZ, _ => SceneNavigationView.NegativeZ })); break;
            case "tool.select": r.ToolPalette?.SetActiveTool(ViewportEditorTool.Select); break;
            case "tool.move": r.ToolPalette?.SetActiveTool(ViewportEditorTool.Move); break;
        }
    }
}
