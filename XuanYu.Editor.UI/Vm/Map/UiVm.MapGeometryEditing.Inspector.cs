using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string InspectorFeatureTypeText => _selectedMapGeometry?.Kind switch
    {
        MapGeometryFeatureKind.Region => "区域",
        MapGeometryFeatureKind.Road => "道路",
        MapGeometryFeatureKind.Marker => "点",
        _ => ""
    };

    public string InspectorFeatureIdText => _selectedMapGeometry?.FeatureId ?? "";

    public string InspectorFeaturePointCountText => _selectedMapGeometry is { } selection
        ? GeometryPoints(selection).Length.ToString()
        : "";

    public string InspectorFeatureClosedText => _selectedMapGeometry?.Kind switch
    {
        MapGeometryFeatureKind.Region => "是",
        MapGeometryFeatureKind.Road or MapGeometryFeatureKind.Marker => "否",
        _ => ""
    };

    public string InspectorFeatureStatusText => _selectedMapGeometry is null ? "" : "已选择";
}
