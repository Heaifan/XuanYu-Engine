using XuanYu.Editor.MapEditing;
using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool CommitSelectedFeatureName(string text)
    {
        EngineResult? result = null;
        if (_selectedMapGeometry is { Kind: MapGeometryFeatureKind.Road } road && MapRoadId.TryParse(road.FeatureId, out var roadId)) result = MapSession.RenameRoad(roadId, text);
        else if (_selectedMapGeometry is { Kind: MapGeometryFeatureKind.Region } region && MapRegionId.TryParse(region.FeatureId, out var regionId)) result = MapSession.RenameRegion(regionId, text);
        else if (_selectedMapGeometry is { Kind: MapGeometryFeatureKind.Marker } marker && MapMarkerId.TryParse(marker.FeatureId, out var markerId)) result = MapSession.RenameMarker(markerId, text);
        FooterMessage = result is { IsSuccess: true } ? "要素名称已提交。" : result?.Error?.Message ?? "要素名称提交失败。";
        RaiseMapGeometryBindings();
        return result?.IsSuccess == true;
    }
}
