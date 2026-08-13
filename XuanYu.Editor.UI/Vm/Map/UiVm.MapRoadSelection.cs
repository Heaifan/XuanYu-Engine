using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TrySelectMapGeometryVertex(double x, double y, ViewportState viewport)
    {
        if (!IsRegionEditMode || (!IsRoadAuthoringMode && !IsMarkerAuthoringMode) || !IsSelectTool ||
            IsRoadDrawingDraftActive || !IsInsideViewport(x, y, viewport)) return false;
        if (!TryMapGeometryVertexHit(x, y, viewport, out var selection, out var index) ||
            (IsRoadAuthoringMode && selection.Kind != MapGeometryFeatureKind.Road) ||
            (IsMarkerAuthoringMode && selection.Kind != MapGeometryFeatureKind.Marker)) return false;
        _selectedMapGeometry = selection;
        _selectedMapGeometryVertexIndex = index;
        _mapGeometryPreview = DisplayGeometry();
        RaiseMapGeometryBindings(); PublishSceneRenderSnapshot();
        return true;
    }
}
