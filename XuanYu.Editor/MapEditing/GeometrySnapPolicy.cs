namespace XuanYu.Editor.MapEditing;

public static class GeometrySnapPolicy
{
    public static bool CanTarget(GeometryFeatureKey source, GeometryFeatureKey target) =>
        source != target && (source.FeatureKind == GeometryFeatureKind.Road ||
            target.FeatureKind == GeometryFeatureKind.Region);

    public static bool IsAllowedSource(GeometryFeatureKey source) =>
        source.FeatureKind is GeometryFeatureKind.Region or GeometryFeatureKind.Road;
}
