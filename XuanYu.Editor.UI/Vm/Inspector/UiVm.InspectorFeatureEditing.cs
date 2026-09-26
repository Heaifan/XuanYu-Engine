using System.Globalization;
using XuanYu.Core.Results;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string InspectorRegionFillColorText =>
        _selectedMapGeometry is { Kind: MapGeometryFeatureKind.Region } selection &&
        MapRegionId.TryParse(selection.FeatureId, out var regionId) &&
        MapSession.CurrentMap.Regions.FirstOrDefault(item => item.RegionId == regionId) is { } region
            ? $"#{region.FillColorRgb & 0x00FFFFFF:X6}"
            : "";

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

    bool CommitRegionFillColor(InspectorEditTarget target, string text)
    {
        if (target.ObjectKind != InspectorObjectKind.Region ||
            !MapRegionId.TryParse(target.ObjectId, out var regionId) ||
            !TryParseRegionColor(text, out var rgb))
        {
            FooterMessage = "区域颜色提交失败：请输入 #RRGGBB。";
            return false;
        }

        var result = MapSession.SetRegionFillColor(regionId, rgb);
        FooterMessage = result.IsSuccess ? "区域填充颜色已提交。" : result.Error?.Message ?? "区域颜色提交失败。";
        if (!result.IsSuccess) return false;
        if (target.ObjectKind == InspectorIdentity) RaiseMapGeometryBindings();
        PublishSceneRenderSnapshot();
        return true;
    }

    static bool TryParseRegionColor(string text, out uint rgb)
    {
        rgb = 0;
        var value = text.Trim();
        if (value.StartsWith('#')) value = value[1..];
        return value.Length == 6 &&
            uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out rgb);
    }
}
