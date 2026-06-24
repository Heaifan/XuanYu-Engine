using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using static XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction.TransformInteractionAction;

namespace XuanYu.Engine.Editor.Windows.Shell.Input.Transform;

/// <summary>变换交互输入路由。负责 G 键 / Esc / Enter 模态、Gizmo Hover、拖拽 Preview 和失焦取消。</summary>
public sealed class EditorTransformInputRoute
{
    public EditorTransformInputResult HandleKeyDown(EditorTransformInputRequest r)
    {
        var sel = r.SelectionRoute.State.SelectedWorldEntity;
        var snap = (r.KeyCode is 0x47 or 0x1B) && sel is not null ? r.BuildTransformSnapshot() : null;
        var kr = TransformKeyboardRoute.HandleKeyDown(r.KeyCode, r.PointerRoute, sel?.EntityId, snap, r.InputState.LastPointerX, r.InputState.LastPointerY);
        if (kr.Action == Started) { r.ToolPalette?.SetActiveTool(ViewportEditorTool.Move); r.InfoLog("G 移动：移动鼠标拖动，左键/Enter 确认，右键/Esc 取消"); return new(true); }
        if (kr.Action == Confirmed) { r.ApplyTransform(kr.Transform, EditorEntityTransformOrigin.MoveTool); r.InfoLog($"移动完成 ({kr.Transform.Position.X:F3}, {kr.Transform.Position.Y:F3}, {kr.Transform.Position.Z:F3})"); return new(true); }
        if (kr.Action == Cancelled) { r.CancelTransform(kr); r.InfoLog("变换已取消"); return new(true); }
        if (kr.Action != NotHandled) return new(true);
        return new(false);
    }

    public EditorTransformInputResult HandlePointerDown(EditorTransformInputRequest r)
    {
        if (!r.PointerRoute.IsBlenderGActive) return new(false);
        r.PointerRoute.SetBlenderGActive(false);
        if (r.ButtonCode == 2) { var c = r.PointerRoute.Cancel(TransformInteractionReason.Escape); r.CancelTransform(c); r.InfoLog("移动已取消"); }
        return new(true);
    }

    public EditorTransformInputResult HandlePointerMoved(EditorTransformInputRequest r)
    {
        if (r.SelectionRoute.State.SelectedWorldEntity is not null) { var g = r.Lifecycle.State.FrameRoute?.Snapshots.PresentedGizmo; if (g?.IsAvailable == true) r.PointerRoute.UpdateGizmoHover(r.X, r.Y, g.Value.Layout); }
        if (r.PointerRoute.IsDragActive) { var dr = r.PointerRoute.OnPointerMoved(r.X, r.Y); if (dr.Action == Previewed) { r.ApplyPreviewPosition(); return new(true); } }
        return new(false);
    }

    public EditorTransformInputResult HandleFocusLost(EditorTransformInputRequest r)
    {
        if (!r.PointerRoute.IsDragActive && !r.PointerRoute.IsBlenderGActive) return new(false);
        r.PointerRoute.SetBlenderGActive(false);
        r.CancelTransform(r.PointerRoute.Cancel(TransformInteractionReason.FocusLost));
        return new(true);
    }
}
