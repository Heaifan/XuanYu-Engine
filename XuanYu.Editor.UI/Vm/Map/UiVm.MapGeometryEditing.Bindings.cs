using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void ClearMapGeometrySelection()
    {
        CancelRegionFillColorPreviewForSelection(null);
        IsGeometryEditingActive = false;
        var hadSelection = _selectedMapGeometry is not null || _selectedMapGeometryVertexIndex >= 0;
        _selectedMapGeometry = null; _selectedMapGeometryVertexIndex = -1; _mapGeometryPreview = null;
        _geometrySnap.Clear();
        MapSession.ClearSelection();
        if (hadSelection) RaiseMapGeometryBindings();
        PublishSceneRenderSnapshot();
    }

    void RaiseMapGeometryBindings()
    {
        OnPropertyChanged(nameof(SelectedMapGeometryText)); OnPropertyChanged(nameof(IsMapGeometryDragActive));
        OnPropertyChanged(nameof(SelectedMapGeometryVertexIndex)); OnPropertyChanged(nameof(IsMapGeometrySelected));
        OnPropertyChanged(nameof(IsRoadGeometrySelected)); OnPropertyChanged(nameof(CanAddSelectedRoadVertex));
        OnPropertyChanged(nameof(CanDeleteSelectedRoadVertex));
        OnPropertyChanged(nameof(InspectorSelectionTitle)); OnPropertyChanged(nameof(InspectorFeatureNameText)); OnPropertyChanged(nameof(InspectorFeatureTypeText));
        OnPropertyChanged(nameof(InspectorFeatureIdText)); OnPropertyChanged(nameof(InspectorFeaturePointCountText));
        OnPropertyChanged(nameof(InspectorFeatureClosedText)); OnPropertyChanged(nameof(InspectorFeatureStatusText));
        RefreshInspectorNavigation();
        RaiseMarkerInspectorBindings();
    }

    public void SelectMapGeometry(MapGeometrySelection selection)
    {
        CancelRegionFillColorPreviewForSelection(selection);
        _selectedMapGeometry = selection; _selectedMapGeometryVertexIndex = -1; IsGeometryEditingActive = false;
        MapSession.ClearSelection();
        _mapGeometryPreview = DisplayGeometry(); RaiseMapGeometryBindings(); PublishSceneRenderSnapshot();
    }
}
