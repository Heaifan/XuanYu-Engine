using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Selection.Ground;
using XuanYu.Engine.Render.Selection.Presented;
using static XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction.TransformInteractionAction;

namespace XuanYu.Engine.Editor.Windows.Shell.Input.Transform;

/// <summary>SceneTool 场景工具输入路由。负责 Gizmo 点按 / 实体体拖拽启动和释放确认。</summary>
public sealed class EditorSceneToolInputRoute
{
    public ViewportSceneToolPressResult HandlePressed(
        int x, int y,
        TransformPointerRoute pointer,
        EditorSelectionRoute selection,
        Scene3dSessionLifecycle lifecycle,
        ViewportPointerPickRoute pick,
        ViewportRenderSceneStore renderScene,
        Func<TransformStartSnapshot?> buildSnapshot,
        Action<string> infoLog)
    {
        if (selection.State.SelectedWorldEntity is null) return ViewportSceneToolPressResult.NotHandled;
        var cam = lifecycle.State.Session?.LastPresentedSnapshot;
        if (cam is not { IsValid: true }) return ViewportSceneToolPressResult.NotHandled;
        var snap = buildSnapshot(); if (snap is null) return ViewportSceneToolPressResult.NotHandled;
        if (pointer.HasHoveredElement)
            return pointer.OnPointerPressed(new(TransformStartSource.GizmoHandle, MoveGizmoElement.ViewPlane, x, y), snap.Value).Action == Started ? ViewportSceneToolPressResult.BeginDrag : ViewportSceneToolPressResult.NotHandled;
        var hit = pick.Pick(new(x, y, cam, lifecycle.State.FrameRoute?.Snapshots.PresentedPick ?? PresentedScenePickSnapshot.None, renderScene.Current, SceneGroundPlane.Default));
        if (hit.Kind == ViewportPickKind.Entity && hit.EntityId == selection.State.SelectedWorldEntity.EntityId)
        { infoLog("实体本体拖动"); return pointer.OnPointerPressed(new(TransformStartSource.EntityBody, MoveGizmoElement.ViewPlane, x, y), snap.Value).Action == Started ? ViewportSceneToolPressResult.BeginDrag : ViewportSceneToolPressResult.NotHandled; }
        return ViewportSceneToolPressResult.NotHandled;
    }

    public TransformInteractionResult HandleReleased(
        TransformPointerRoute pointer,
        Action<SceneTransform, EditorEntityTransformOrigin> applyTransform,
        Action<string> infoLog)
    {
        var result = pointer.OnPointerReleased();
        if (result.Action == Confirmed)
        { applyTransform(result.Transform, EditorEntityTransformOrigin.MoveTool); infoLog($"移动完成 ({result.Transform.Position.X:F3}, {result.Transform.Position.Y:F3}, {result.Transform.Position.Z:F3})"); }
        return result;
    }
}
