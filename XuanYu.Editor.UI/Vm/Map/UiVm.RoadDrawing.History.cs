namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool CanUndoRoadDrawingVertex => _roadDrawing.CanUndo;
    public bool CanRedoRoadDrawingVertex => _roadDrawing.CanRedo;
    public bool CanCompleteRoadDrawing => _roadDrawing.IsActive && _roadDrawing.Draft?.CanComplete == true;
    public bool CanCancelRoadDrawing => _roadDrawing.IsActive;
    public bool UndoRoadDrawingVertex() { if (!_roadDrawing.UndoVertex()) return false; RaiseRoadDrawingBindings(); PublishSceneRenderSnapshot(); return true; }
    public bool RedoRoadDrawingVertex() { if (!_roadDrawing.RedoVertex()) return false; RaiseRoadDrawingBindings(); PublishSceneRenderSnapshot(); return true; }
    public bool CompleteRoadDrawing() => CommitRoadDrawingFromEnter();
    public bool CancelRoadDrawing() => CancelRoadDrawingFromEscape();
    void RaiseRoadDrawingBindings()
    {
        OnPropertyChanged(nameof(IsRoadDrawingDraftActive)); OnPropertyChanged(nameof(RoadDrawingDraftPointCount)); OnPropertyChanged(nameof(RoadDrawingDraftStatus));
        OnPropertyChanged(nameof(CanUndoRoadDrawingVertex)); OnPropertyChanged(nameof(CanRedoRoadDrawingVertex)); OnPropertyChanged(nameof(CanCompleteRoadDrawing)); OnPropertyChanged(nameof(CanCancelRoadDrawing)); OnPropertyChanged(nameof(RoadContentCount));
    }
}
