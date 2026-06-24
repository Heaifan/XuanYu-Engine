namespace XuanYu.Engine.Editor.Windows.Shell.Composition;

/// <summary>EditorShell 生命周期路由。处理 Attach/Detach 事件转发。</summary>
sealed class EditorShellLifecycle(EditorShellContext ctx)
{
    public EditorShellAttachRequest BuildAttachRequest() => new(
        NativeHostReportAction: ctx.StartupProbeRoute.RunAttachedProbes,
        InputPipelineInitAction: InitializeInputPipeline);

    public EditorShellDetachRequest BuildDetachRequest() => new(
        Lifecycle: ctx.Lifecycle,
        ResizeRenderTimer: ctx.ViewportRedrawRoute.Timer,
        ResizeRenderTimerTickHandler: ctx.ViewportRedrawRoute.TimerTickHandler);

    public void ApplyDetachResult(EditorShellDetachResult result)
    {
        ctx.SessionActive = false;
        ctx.StartupVulkanRoute.Reset();
        if (result.TimerCleanedUp) ctx.ViewportRedrawRoute.ClearTimer();
    }

    void InitializeInputPipeline()
    {
        EditorInputService.Instance.Initialize();
        ctx.ViewportInputRoute.State.Translator = new WindowsViewportInputTranslator(EditorInputService.Instance.CurrentSnapshot);
        EditorInputService.Instance.SnapshotReplaced += snapshot =>
            ctx.ViewportInputRoute.State.Translator?.OnSnapshotReplaced(snapshot);
    }
}
