namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool CanUndoRoadDrawingVertex => RoadDrawingController.CanUndo;
    public bool CanRedoRoadDrawingVertex => RoadDrawingController.CanRedo;
    public bool CanCompleteRoadDrawing => RoadDrawingController.CanComplete;
    public bool CanCancelRoadDrawing => RoadDrawingController.IsActive;
    public bool UndoRoadDrawingVertex() { if (!RoadDrawingController.Undo()) return false; _roadDrawing.ProjectFrom(RoadDrawingController.Session, RoadDrawingController.Metadata); RaiseRoadDrawingBindings(); PublishSceneRenderSnapshot(); return true; }
    public bool RedoRoadDrawingVertex() { if (!RoadDrawingController.Redo()) return false; _roadDrawing.ProjectFrom(RoadDrawingController.Session, RoadDrawingController.Metadata); RaiseRoadDrawingBindings(); PublishSceneRenderSnapshot(); return true; }
    public bool CompleteRoadDrawing() => CommitRoadDrawingFromEnter();
    public bool CancelRoadDrawing() => CancelRoadDrawingFromEscape();
    void RaiseRoadDrawingBindings()
    {
        OnPropertyChanged(nameof(IsRoadDrawingDraftActive)); OnPropertyChanged(nameof(RoadDrawingDraftPointCount)); OnPropertyChanged(nameof(RoadDrawingDraftStatus));
        OnPropertyChanged(nameof(CanUndoRoadDrawingVertex)); OnPropertyChanged(nameof(CanRedoRoadDrawingVertex)); OnPropertyChanged(nameof(CanCompleteRoadDrawing)); OnPropertyChanged(nameof(CanCancelRoadDrawing)); OnPropertyChanged(nameof(RoadContentCount));
        RaiseContextToolbarDrawingBindings();
    }
}
