using XuanYu.Editor.MapEditing;
using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool CommitFeatureName(InspectorEditTarget target, string text)
    {
        EngineResult? result = null;
        if (target.ObjectKind == InspectorObjectKind.Road && MapRoadId.TryParse(target.ObjectId, out var roadId)) result = MapSession.RenameRoad(roadId, text);
        else if (target.ObjectKind == InspectorObjectKind.Region && MapRegionId.TryParse(target.ObjectId, out var regionId)) result = MapSession.RenameRegion(regionId, text);
        else if (target.ObjectKind == InspectorObjectKind.Marker && MapMarkerId.TryParse(target.ObjectId, out var markerId)) result = MapSession.RenameMarker(markerId, text);
        FooterMessage = result is { IsSuccess: true } ? "要素名称已提交。" : result?.Error?.Message ?? "要素名称提交失败。";
        if (target.ObjectKind == InspectorIdentity) RaiseMapGeometryBindings();
        return result?.IsSuccess == true;
    }
}
