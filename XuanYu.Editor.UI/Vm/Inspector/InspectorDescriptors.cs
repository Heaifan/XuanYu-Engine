namespace XuanYu.Editor.UI;

static class InspectorDescriptors
{
    public static IReadOnlyList<InspectorPropertyDescriptor> For(UiVm vm, InspectorObjectKind kind) => kind switch
    {
        InspectorObjectKind.Entity => [D("Entity.Basic.Name", kind, "基础", "名称", "名称"), D("Entity.Geometry.Position", kind, "几何", "变换", "位置"),
            D("Entity.Geometry.Rotation", kind, "几何", "变换", "旋转"), D("Entity.Geometry.Scale", kind, "几何", "变换", "缩放")],
        InspectorObjectKind.Map => [D("Map.Basic.Name", kind, "基础", "地图", "地图名称"), D("Map.Geometry.Size", kind, "几何", "尺寸", "地图尺寸")],
        InspectorObjectKind.Road => Feature(kind, "道路"),
        InspectorObjectKind.Region => Feature(kind, "区域"),
        InspectorObjectKind.Marker => Feature(kind, "点"),
        _ => []
    };

    static IReadOnlyList<InspectorPropertyDescriptor> Feature(InspectorObjectKind kind, string name) =>
        [D($"{kind}.Basic.Name", kind, "基础", "标识", $"{name}名称"), D($"{kind}.Geometry.Points", kind, "几何", "形状", "节点数量"), D($"{kind}.Status.State", kind, "状态", "可见性", "状态")];

    static InspectorPropertyDescriptor D(string key, InspectorObjectKind kind, string category, string section, string name) =>
        new(key, kind, category, section, name);

    public static string ValueFor(UiVm vm, string key) => key switch
    {
        "Entity.Basic.Name" => vm.InspectorEntityNameText,
        "Entity.Geometry.Position" => $"X {vm.InspectorPositionX:0.##}  Y {vm.InspectorPositionY:0.##}  Z {vm.InspectorPositionZ:0.##}",
        "Entity.Geometry.Rotation" => $"X {vm.InspectorRotationX:0.##}  Y {vm.InspectorRotationY:0.##}  Z {vm.InspectorRotationZ:0.##}",
        "Entity.Geometry.Scale" => $"X {vm.InspectorScaleX:0.##}  Y {vm.InspectorScaleY:0.##}  Z {vm.InspectorScaleZ:0.##}",
        "Map.Basic.Name" => vm.MapName,
        "Map.Geometry.Size" => vm.MapSizeText,
        _ when key.Contains(".Geometry.Points", StringComparison.Ordinal) => vm.InspectorFeaturePointCountText,
        _ when key.Contains(".Status.State", StringComparison.Ordinal) => vm.InspectorFeatureStatusText,
        _ => vm.InspectorSelectionTitle
    };
}
