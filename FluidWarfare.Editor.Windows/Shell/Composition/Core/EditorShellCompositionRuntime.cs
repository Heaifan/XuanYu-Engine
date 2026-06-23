namespace FluidWarfare.Editor.Windows.Shell.Composition;

/// <summary>EditorShell 运行时辅助方法。Build 方法调用的本地函数提取。</summary>
sealed class EditorShellCompositionRuntime(EditorShellContext ctx)
{
    public void ScheduleFrame(VulkanScene3dFrameReason reason) =>
        ctx.DiagnosticsRoute.ScheduleFrame(reason, ctx.RenderSeq, ctx.SelectionRoute, ctx.WorldState,
            () => { ctx.RenderSeq = ctx.Lifecycle.State.FrameSubmitRoute?.RenderSeq ?? ctx.RenderSeq; ctx.LogRoute.RefreshDiagnostics(); });

    public EditorViewportInputRequest BuildInputRequest(EditorViewportInputKind kind, int kc = 0, int bc = 0, int x = 0, int y = 0, int wd = 0) =>
        new(kind, kc, bc, x, y, wd, ctx.ViewportInputRoute.State, ctx.PointerRoute, ctx.SelectionRoute,
            ctx.ToolPalette, ctx.CameraRoute, ctx.Lifecycle, ctx.ViewportPickRoute, ctx.RenderSceneStore,
            ctx.GroundPlacementState, ctx.WorldDirtyState, ctx.LogRoute.Info, ctx.LogRoute.Warn,
            ScheduleFrame, BuildTransformStartSnapshot, ctx.TransformRoute.ApplyEntityTransform,
            CancelActiveTransform, ApplyPreviewPosition, ctx.ViewportFrameRoute.ExecuteViewportFrameSelected);

    public EditorScene3dCommandRequest BuildScene3dCmdRequest(EditorScene3dCommandKind kind) =>
        new(kind, ctx.ProbeRoute, ctx.Lifecycle, ctx.RenderSceneStore,
            ctx.VulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            ctx.CameraRoute, ctx.RenderSeq, ctx.LogRoute.Info, ctx.LogRoute.Warn);

    public void ApplyScene3dCommandResult(EditorScene3dCommandResult r)
    {
        if (r.SessionStarted) { ctx.SessionActive = true; ctx.RenderLastMode = "Scene3D"; ctx.RenderSeq = r.NewRenderSeq; InitTransformApplication(); }
        if (!r.SessionStarted && r.NeedsTransformInit) ctx.SessionActive = false;
        if (r.NeedsDiagnosticsRefresh) ctx.LogRoute.RefreshDiagnostics();
        if (r.NewRenderSeq > ctx.RenderSeq) ctx.RenderSeq = r.NewRenderSeq;
    }

    public void ApplyScene3dCommandResult_Direct()
    {
        var req = BuildScene3dCmdRequest(EditorScene3dCommandKind.Restart);
        ApplyScene3dCommandResult(ctx.Scene3dCommandRoute.Execute(req));
    }

    void InitTransformApplication()
    {
        if (ctx.Lifecycle.State.Session is null) return;
        var vulkan = new Scene3dEntityPositionWriter(ctx.Lifecycle.State.Session);
        var inspect = new InspectorTransformDisplay(ctx.InspectorPanel);
        ctx.PreviewApplier = new EntityTransformPreview(ctx.RenderSceneStore, vulkan, inspect);
        ctx.CancelApplier = new EntityTransformCancel(ctx.RenderSceneStore, vulkan, inspect);
        if (ctx.WorldState is not null)
        {
            var world = new WorldTransformWriter(ctx.WorldState, ctx.WorldDirtyState, ctx.StatusBarPanel);
            ctx.CommitApplier = new EntityTransformCommit(world, ctx.RenderSceneStore, vulkan, inspect);
        }
    }

    TransformStartSnapshot? BuildTransformStartSnapshot()
    {
        if (ctx.SelectionRoute.State.SelectedWorldEntity is null) return null;
        var pos = ctx.WorldState?.FindPosition(ctx.SelectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return null;
        var cam = ctx.Lifecycle.State.Session?.LastPresentedSnapshot;
        if (cam is not { IsValid: true }) return null;
        return new(ctx.SelectionRoute.State.SelectedWorldEntity.EntityId, new(pos.Value.Value, default, default),
            ctx.WorldDirtyState.IsDirty, cam,
            ctx.Lifecycle.State.FrameRoute?.Snapshots.PresentedGizmo ?? PresentedMoveGizmoSnapshot.None);
    }

    void ApplyPreviewPosition() => ctx.TransformApplyRoute.Preview(ctx.SelectionRoute, ctx.PreviewApplier, ctx.PointerRoute, ScheduleFrame);
    void CancelActiveTransform(TransformInteractionResult tr) => ctx.TransformApplyRoute.Cancel(tr, ctx.SelectionRoute, ctx.CancelApplier, ScheduleFrame);

    public void CompleteGroundPlacement(Vector3d gp)
    {
        var r2 = ctx.GroundPlacementRoute.Complete(gp, ctx.SelectionRoute, ctx.GroundPlacementState,
            ctx.WorldState, ctx.CommitApplier, ScheduleFrame, ctx.InspectorPanel, ctx.LogRoute.Info);
        if (r2.Completed) ctx.PickingRoute.HideGroundCursor();
    }
}
