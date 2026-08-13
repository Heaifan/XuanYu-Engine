using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ClearMapGeometrySelection()
    {
        if (_selectedMapGeometry is null && _selectedMapGeometryVertexIndex < 0) return;
        _selectedMapGeometry = null; _selectedMapGeometryVertexIndex = -1; _mapGeometryPreview = null;
        _geometrySnap.Clear();
        RaiseMapGeometryBindings(); PublishSceneRenderSnapshot();
    }

    void RaiseMapGeometryBindings()
    {
        OnPropertyChanged(nameof(SelectedMapGeometryText)); OnPropertyChanged(nameof(IsMapGeometryDragActive));
        OnPropertyChanged(nameof(SelectedMapGeometryVertexIndex));
    }

    void SelectMapGeometry(MapGeometrySelection selection)
    {
        _selectedMapGeometry = selection; _selectedMapGeometryVertexIndex = -1;
        _mapGeometryPreview = DisplayGeometry(); RaiseMapGeometryBindings(); PublishSceneRenderSnapshot();
    }
}
