namespace XuanYu.Editor.MapEditing;

public static class GeometrySnapPolicy
{
    public static bool CanTarget(GeometryFeatureKey source, GeometryFeatureKey target) => source != target &&
        source.FeatureKind switch
        {
            GeometryFeatureKind.Region => target.FeatureKind is GeometryFeatureKind.Region or GeometryFeatureKind.Marker,
            GeometryFeatureKind.Road or GeometryFeatureKind.Marker => target.FeatureKind is GeometryFeatureKind.Region or GeometryFeatureKind.Road or GeometryFeatureKind.Marker,
            _ => false
        };

    public static bool IsAllowedSource(GeometryFeatureKey source) =>
        source.FeatureKind is GeometryFeatureKind.Region or GeometryFeatureKind.Road or GeometryFeatureKind.Marker;
}
