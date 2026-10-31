namespace XuanYu.Editor.MapEditing;

public enum GeometryFeatureKind { Region, Road, Marker }

public readonly record struct GeometryFeatureKey(
    GeometryFeatureKind FeatureKind, string FeatureId)
{
    public override string ToString() => $"{FeatureKind}:{FeatureId}";
}
