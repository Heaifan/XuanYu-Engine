using XuanYu.Engine.Editor.Input.Actions;
using XuanYu.Engine.Editor.Input.Runtime;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.Editor.Windows.Shell.Input.Transform;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
namespace XuanYu.Engine.Editor.Windows.Shell.Input;
public sealed class EditorViewportInputRoute
{
    public EditorViewportInputState State { get; } = new();
    public EditorTransformInputRoute TransformInput { get; } = new();
    public EditorSceneToolInputRoute SceneToolInput { get; } = new();

    EditorTransformInputRequest TReq(EditorViewportInputRequest r, int kc = 0, int bc = 0, int x = 0, int y = 0) =>
        new(kc != 0 ? kc : r.KeyCode, bc != 0 ? bc : r.ButtonCode, x != 0 ? x : r.X, y != 0 ? y : r.Y,
            r.State, r.PointerRoute, r.SelectionRoute, r.ToolPalette, r.Lifecycle,
            r.InfoLog, r.ApplyTransform, r.CancelTransform, r.ApplyPreviewPosition, r.BuildTransformSnapshot);

    public EditorViewportInputResult HandleKeyDown(EditorViewportInputRequest r)
    {
        if (TransformInput.HandleKeyDown(TReq(r)).Handled) return new(true);
        return Trans(r, () => r.State.Translator?.OnRawKeyDown(r.KeyCode, r.State.LastPointerX, r.State.LastPointerY));
    }
    public EditorViewportInputResult HandleKeyUp(EditorViewportInputRequest r) { r.State.Translator?.OnRawKeyUp(r.KeyCode); return new(false); }
    public EditorViewportInputResult HandlePointerDown(EditorViewportInputRequest r)
    {
        r.State.LastPointerX = r.X; r.State.LastPointerY = r.Y;
        if (TransformInput.HandlePointerDown(TReq(r)).Handled) return new(true);
        return Trans(r, () => r.State.Translator?.OnRawPointerButtonDown(r.ButtonCode, r.X, r.Y));
    }
    public EditorViewportInputResult HandlePointerMoved(EditorViewportInputRequest r)
    {
        using var probe = GizmoDragProbe.BeginFrame("PointerMoved");
        GizmoDragProbe.Log("PointerMoved入口");
        r.State.LastPointerX = r.X; r.State.LastPointerY = r.Y;
        if (TransformInput.HandlePointerMoved(TReq(r, x: r.X, y: r.Y)).Handled) return new(true);
        return Trans(r, () => r.State.Translator?.OnRawPointerMoved(r.X, r.Y));
    }
    public EditorViewportInputResult HandlePointerUp(EditorViewportInputRequest r) { r.State.Translator?.OnRawPointerButtonUp(r.ButtonCode); return new(false); }
    public EditorViewportInputResult HandleMouseWheel(EditorViewportInputRequest r) { return Trans(r, () => r.State.Translator?.OnRawMouseWheel(r.WheelDelta, 0, r.State.LastPointerX, r.State.LastPointerY)); }
    public EditorViewportInputResult HandleFocusLost(EditorViewportInputRequest r)
    {
        r.State.Translator?.OnRawInputFocusLost();
        if (TransformInput.HandleFocusLost(TReq(r)).Handled) return new(true);
        return new(true);
    }
    public ViewportSceneToolPressResult HandleSceneToolPressed(EditorViewportInputRequest r) =>
        SceneToolInput.HandlePressed(r.X, r.Y, r.PointerRoute, r.SelectionRoute, r.Lifecycle,
            r.PickRoute, r.RenderSceneStore, r.BuildTransformSnapshot, r.InfoLog);
    public TransformInteractionResult HandleSceneToolReleased(EditorViewportInputRequest r) =>
        SceneToolInput.HandleReleased(r.PointerRoute, r.ApplyTransform, r.InfoLog);

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
