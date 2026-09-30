namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void EndRegionDrawingAfterToolChange()
    {
        var hadDraft = _regionDrawing.IsActive;
        if (!IsDrawingTransactionActive && !hadDraft) return;
        _regionDrawing.Cancel();
        ClearRegionDrawingSnap();
        ClearMapGeometrySelection(clearInspectorSelection: false);
        RaiseRegionDrawingBindings();
        EndDrawingTransaction();
        if (hadDraft) LogRegionDrawingCanceled();
    }

    void EndRoadDrawingAfterToolChange()
    {
        var hadDraft = _roadDrawing.IsActive;
        if (!IsDrawingTransactionActive && !hadDraft) return;
        _roadDrawing.Cancel();
        RaiseRoadDrawingBindings();
        EndDrawingTransaction();
        if (hadDraft) LogRoadDrawingCanceled();
    }
}
