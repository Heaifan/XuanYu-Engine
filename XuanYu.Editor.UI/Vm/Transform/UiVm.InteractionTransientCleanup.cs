using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    internal void ClearTransientInteractionState(EditorPointerEventKind reason)
    {
        var hadMapTransient = _mapGeometryDrag is not null ||
            _mapGeometryPreview is not null || _regionVertexSnap.IsSnapped ||
            _geometrySnap.IsSnapped || _selectedMapGeometryVertexIndex >= 0;
        var hadDrawingTransient = IsDrawingTransactionActive || _regionDrawing.IsActive ||
            RoadDrawingController.IsActive || _roadDrawing.IsActive || _regionDrawingSnap.IsSnapped;
        _moveDragConstraint = null;
        _rotateDrag = null;
        _scaleDrag = null;
        ClearNavigationGizmoInteractionState();
        _regionDrawing.Cancel();
        RoadDrawingController.Cancel();
        _roadDrawing.Clear();
        ClearRegionDrawingSnap();
        _mapGeometryDrag = null;
        _selectedMapGeometryVertexIndex = -1;
        _mapGeometryPreview = null;
        _regionVertexSnap.Clear();
        _geometrySnap.Clear();
        if (hadMapTransient) RaiseMapGeometryBindings();
        if (hadDrawingTransient)
        {
            EndDrawingTransaction();
            RaiseRegionDrawingBindings();
            RaiseRoadDrawingBindings();
        }
    }
}
