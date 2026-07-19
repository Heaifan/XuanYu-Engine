namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string InteractionPhase => _editorState.InteractionSnapshot.PhaseText;
    public string InteractionOwner => _editorState.InteractionSnapshot.OwnerTool is "" ? "无" : _editorState.InteractionSnapshot.OwnerTool;
    public string InteractionPreview => _editorState.InteractionSnapshot.Preview is "" ? "无" : _editorState.InteractionSnapshot.Preview;

    public void CancelInteractionFromEscape() => CancelInteraction("Escape");
    public void CancelInteractionFromWindowDeactivated() => CancelInteraction("窗口失焦");
    public void CancelInteractionFromWindowClosing() => CancelInteraction("窗口关闭");
    public void CancelInteractionFromHostDetach() => CancelInteraction("NativeHost Detach");
    public void CancelInteractionFromPointerCaptureLost() => CancelInteraction("PointerCaptureLost");
    public void CancelInteractionFromNativePointer(string reason) => CancelInteraction(reason);

    void RunInteraction(string name)
    {
        if (name == "Begin") BeginInteraction();
        else if (name == "Preview") PreviewInteraction();
        else if (name == "Commit") CommitInteraction();
        else if (name == "Cancel") CancelInteraction("手动取消");
    }

    void BeginInteraction()
    {
        var result = _editorState.Begin(new BeginInteractionCommand(ActiveTool,
            SelectionTitle, EditorInteractionPointerSnapshot.Empty));
        if (result is null) return;
        FooterState = "状态：捕获中";
        FooterMessage = $"交互开始：{ActiveTool}";
        LogInteraction("开始捕获", $"Session={result.Snapshot.SessionId}");
        RaiseInteractionChanged();
    }

    void PreviewInteraction()
    {
        var snap = _editorState.InteractionSnapshot;
        var pointer = snap.Pointer.IsEmpty ? snap.Pointer : snap.Pointer.MoveTo(
            snap.Pointer.PointerId, snap.Pointer.CurrentX, snap.Pointer.CurrentY);
        var result = _editorState.Preview(new PreviewInteractionCommand(
            snap.SessionId, snap.OwnerTool, $"预览 {snap.Revision}", pointer));
        if (result is null) return;
        FooterMessage = $"交互预览：{InteractionPreview}";
        RaiseInteractionChanged();
    }

    void CommitInteraction()
    {
        var snap = _editorState.InteractionSnapshot;
        var result = _editorState.Commit(new CommitInteractionCommand(
            snap.SessionId, snap.OwnerTool, snap.Pointer.PointerId));
        if (result is null) return;
        var transformCommitted = _transformSession.TryCommit(snap.SessionId, _sceneState);
        _moveDragConstraint = null;
        if (transformCommitted) PublishSceneRenderSnapshot();
        FooterState = "状态：就绪";
        FooterMessage = "交互已提交。";
        LogInteraction("提交捕获", $"Session={snap.SessionId}；{snap.Pointer.Summary}");
        LogMoveGizmoEnd("结束", snap);
        RaiseInteractionChanged();
    }

    void CancelInteraction(string reason)
    {
        var snap = _editorState.InteractionSnapshot;
        var result = _editorState.Cancel(new CancelInteractionCommand(snap.SessionId, snap.OwnerTool, reason));
        if (result is null) return;
        var transformCanceled = _transformSession.TryCancel(snap.SessionId);
        _moveDragConstraint = null;
        if (transformCanceled) PublishSceneRenderSnapshot();
        FooterState = "状态：就绪";
        FooterMessage = $"交互已取消：{reason}";
        LogInteraction("取消捕获", $"Session={snap.SessionId}，原因={reason}");
        LogMoveGizmoEnd($"取消，原因={reason}", snap);
        RaiseInteractionChanged();
    }

    void RaiseInteractionChanged()
    {
        OnPropertyChanged(nameof(InteractionPhase));
        OnPropertyChanged(nameof(InteractionOwner));
        OnPropertyChanged(nameof(InteractionPreview));
        OnPropertyChanged(nameof(DebugInputItems));
        OnPropertyChanged(nameof(LogSummary));
    }

    void LogMoveGizmoEnd(string result, EditorInteractionSnapshot snap)
    {
        if (snap.OwnerTool != "移动" || !snap.StartSnapshot.StartsWith("Entity=", StringComparison.Ordinal)) return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            $"【ARCH-C-R5】移动工具会话{result}",
            $"{snap.StartSnapshot}; Session={snap.SessionId}; Position={_sceneState.RenderSnapshot.Entity.Transform.Position}");
        RefreshLogBindings();
    }
}
