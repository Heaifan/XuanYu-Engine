using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    RegionFillColorPreview? _regionFillColorPreview;

    public bool BeginRegionFillColorPreview(InspectorEditTarget target)
    {
        if (target.ObjectKind != InspectorObjectKind.Region ||
            target.PropertyKey != "Region.Style.FillColor" ||
            !MapRegionId.TryParse(target.ObjectId, out var regionId))
            return false;
        var region = MapSession.CurrentMap.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return false;
        _regionFillColorPreview = new(target, region.FillColorRgb, region.FillColorRgb);
        return true;
    }

    public void PreviewRegionFillColor(InspectorEditTarget target, uint rgb)
    {
        if (_regionFillColorPreview is not { } active || active.Target != target) return;
        var normalized = rgb & 0x00FFFFFF;
        if (active.PreviewRgb == normalized) return;
        _regionFillColorPreview = active with { PreviewRgb = normalized };
        PublishSceneRenderSnapshot();
    }

    public bool CommitRegionFillColorPreview(InspectorEditTarget target, uint rgb)
    {
        if (_regionFillColorPreview is not { } active || active.Target != target) return false;
        _regionFillColorPreview = null;
        var succeeded = CommitInspectorProperty(target, $"#{rgb & 0x00FFFFFF:X6}");
        if (!succeeded) PublishSceneRenderSnapshot();
        return succeeded;
    }

    public void CancelRegionFillColorPreview(InspectorEditTarget target)
    {
        if (_regionFillColorPreview is not { } active || active.Target != target) return;
        _regionFillColorPreview = null;
        PublishSceneRenderSnapshot();
    }

    void CancelRegionFillColorPreviewForSelection(MapGeometrySelection? selection)
    {
        if (_regionFillColorPreview is not { } active) return;
        if (selection is { Kind: MapGeometryFeatureKind.Region } &&
            selection.Value.FeatureId == active.Target.ObjectId) return;
        _regionFillColorPreview = null;
    }

    void CancelRegionFillColorPreviewForMapSelection(MapSelection selection)
    {
        MapGeometrySelection? geometry = selection.Kind == MapSelectionKind.Region && selection.RegionId is { } regionId
            ? new MapGeometrySelection(MapGeometryFeatureKind.Region, regionId.ToString())
            : null;
        CancelRegionFillColorPreviewForSelection(geometry);
    }

    void CancelRegionFillColorPreviewForContentChange() => _regionFillColorPreview = null;

    MapDefinition RegionFillColorPreviewMap() =>
        MapRegionColorPreviewProjection.Apply(MapSession.CurrentMap, _regionFillColorPreview);
}
