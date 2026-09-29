namespace XuanYu.World.Map;

public enum SurfaceBindingKind
{
    ReferencePlane,
    Terrain
}

public sealed record SurfaceBinding(SurfaceBindingKind Kind, string Identity)
{
    public static SurfaceBinding ReferencePlane { get; } =
        new(SurfaceBindingKind.ReferencePlane, "reference-plane");

    public static SurfaceBinding Terrain(string identity) =>
        new(SurfaceBindingKind.Terrain, string.IsNullOrWhiteSpace(identity)
            ? throw new ArgumentException("Terrain identity 不能为空。", nameof(identity))
            : identity);
}
