namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool CanUndoRegionDrawingVertex => _regionDrawing.CanUndo;
    public bool CanRedoRegionDrawingVertex => _regionDrawing.CanRedo;
    public bool CanCompleteRegionDrawing => _regionDrawing.IsActive && _regionDrawing.Draft?.CanClose == true;
    public bool CanCancelRegionDrawing => _regionDrawing.IsActive;

    public bool UndoRegionDrawingVertex()
    {
        if (!_regionDrawing.UndoVertex()) return false;
        ClearRegionDrawingSnap();
        FooterMessage = "已撤销上一个区域顶点。";
        RaiseRegionDrawingBindings(); PublishSceneRenderSnapshot();
        return true;
    }

    public bool RedoRegionDrawingVertex()
    {
        if (!_regionDrawing.RedoVertex()) return false;
        ClearRegionDrawingSnap();
        FooterMessage = "已重做区域顶点。";
        RaiseRegionDrawingBindings(); PublishSceneRenderSnapshot();
        return true;
    }

    public bool CompleteRegionDrawing() => CommitRegionDrawingFromEnter();

    public bool CancelRegionDrawing() => CancelRegionDrawingFromEscape();

    void RaiseRegionDrawingBindings()
    {
        OnPropertyChanged(nameof(IsRegionDrawingDraftActive));
        OnPropertyChanged(nameof(IsRegionDrawingCloseCandidate));
        OnPropertyChanged(nameof(RegionDrawingDraftVertexCount));
        OnPropertyChanged(nameof(RegionDrawingDraftStatus));
        OnPropertyChanged(nameof(CanUndoRegionDrawingVertex));
        OnPropertyChanged(nameof(CanRedoRegionDrawingVertex));
        OnPropertyChanged(nameof(CanCompleteRegionDrawing));
        OnPropertyChanged(nameof(CanCancelRegionDrawing));
        OnPropertyChanged(nameof(RegionContentCount));
        OnPropertyChanged(nameof(IsRegionDrawingSnapActive));
        OnPropertyChanged(nameof(RegionDrawingSnapTargetRegionId));
        OnPropertyChanged(nameof(RegionDrawingSnapTargetVertexIndex));
        OnPropertyChanged(nameof(RegionDrawingSnapTargetPoint));
        OnPropertyChanged(nameof(RegionDrawingCursor));
        OnPropertyChanged(nameof(RegionDrawingSnapStatus));
        RaiseContextToolbarDrawingBindings();
    }
}
