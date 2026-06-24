using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Submit;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Resize;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.World;
using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Diagnostics;

public sealed class EditorDiagnosticsRefreshRoute
{
    public EditorDiagnosticsRefreshState State { get; } = new();
    EditorDiagnosticsContext _ctx = null!;
    public void SetContext(EditorDiagnosticsContext ctx) => _ctx = ctx;

    public void Refresh(bool sessionActive, string renderLastMode)
    {
        var ps = _ctx.ProbeRoute.State; var nh = _ctx.GetNativeHostInfo(); var s3d = ps.Scene3d;
        _ctx.Feedback.RefreshViewportStatusLine(sessionActive, _ctx.Lifecycle.State, ps, renderLastMode);
        _ctx.Feedback.RefreshAllDiagnostics(ps, nh, _ctx.RenderSceneStore.Current.Objects, s3d.IsSucceeded, s3d.Message, s3d.CameraSummary,
            s3d.GridVertexCount, s3d.GridLineCount, s3d.UnitVertexCount, s3d.UnitTriangleCount,
            s3d.RenderedUnitCount, s3d.RenderObjectCount, s3d.IgnoredObjectCount, s3d.DrawCallCount, s3d.DepthFormat,
            s3d.DepthAttachmentCount, s3d.DepthTestEnabled, ps.Instance.ElapsedMilliseconds, ps.Device.ElapsedMilliseconds,
            ps.Swapchain.ElapsedMilliseconds, ps.Clear.ElapsedMilliseconds, s3d.ElapsedMilliseconds);
        _ctx.RunMenu.SetScene3dEnabled(VulkanScene3dRunGate.Evaluate().CanRun);
        _ctx.StatusBar?.SetVulkanStatus(ps.Backend.IsAvailable ? "已接入" : "不可用");
    }

    public void ScheduleFrame(VulkanScene3dFrameReason reason, int renderSeq,
        EditorSelectionRoute selection, WorldState? world, Action refresh)
    {
        var submit = _ctx.Lifecycle.State.FrameSubmitRoute; if (submit is null) return;
        var sel = selection.State.SelectedWorldEntity;
        var entityPos = _ctx.PointerRoute.Session.IsActive ? _ctx.PointerRoute.Session.PreviewTransform.Position
            : sel is not null ? world?.FindPosition(sel.EntityId)?.Value ?? Vector3d.Zero : Vector3d.Zero;
        submit.Request(new Scene3dFrameSubmitInput(reason, _ctx.CameraRoute.LastCameraState,
            _ctx.CameraRoute.CameraRevision, renderSeq, _ctx.PointerRoute.IsMoveToolActive,
            sel?.EntityId ?? default, entityPos, _ctx.PointerRoute.HoveredElement,
            _ctx.WorldDirtyState.Revision), () => refresh());
    }

    public int ApplyResizeResult(Scene3dResizeRenderResult result, Action<string> info, Action<string> warn)
    { if (result.LogMessage is not null) { if (result.LogIsWarning) warn(result.LogMessage); else info(result.LogMessage); } return result.NewRenderSeq; }

    public void ProbeValidation(Action<string> info, Action<string> warn)
    { _ctx.ProbeRoute.ProbeValidation(info, warn); Refresh(false, "无"); }

    public void UpdateViewportHost()
    { _ctx.VulkanHost?.ShowClearStatus(_ctx.ProbeRoute.State.Backend.IsAvailable ? "Vulkan 后端就绪，等待 Surface/Swapchain。" : "Vulkan 后端不可用。"); }
}
