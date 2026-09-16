using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void ClearMapGeometrySelection()
    {
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
        OnPropertyChanged(nameof(InspectorSelectionTitle)); OnPropertyChanged(nameof(InspectorFeatureNameText)); OnPropertyChanged(nameof(InspectorFeatureTypeText));
        OnPropertyChanged(nameof(InspectorFeatureIdText)); OnPropertyChanged(nameof(InspectorFeaturePointCountText));
        OnPropertyChanged(nameof(InspectorFeatureClosedText)); OnPropertyChanged(nameof(InspectorFeatureStatusText));
        RefreshInspectorNavigation();
        RaiseMarkerInspectorBindings();
    }

    public void SelectMapGeometry(MapGeometrySelection selection)
    {
        _selectedMapGeometry = selection; _selectedMapGeometryVertexIndex = -1; IsGeometryEditingActive = false;
        MapSession.ClearSelection();
        _mapGeometryPreview = DisplayGeometry(); RaiseMapGeometryBindings(); PublishSceneRenderSnapshot();
    }
}
