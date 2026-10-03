namespace XuanYu.World;

public readonly record struct SurfaceQueryResult(
    WorldQueryStatus Status, double? Value)
{
    public bool IsValid => Status == WorldQueryStatus.Valid;

    public double SurfaceZ => IsValid
        ? Value!.Value
        : throw new InvalidOperationException("无效查询没有地表高度值。");

    public static SurfaceQueryResult Valid(double surfaceZ) =>
        new(WorldQueryStatus.Valid, surfaceZ);

    public static SurfaceQueryResult NoData =>
        new(WorldQueryStatus.NoData, null);

    public static SurfaceQueryResult OutOfBounds =>
        new(WorldQueryStatus.OutOfBounds, null);

    public static SurfaceQueryResult NoTerrain =>
        new(WorldQueryStatus.NoTerrain, null);

    public static SurfaceQueryResult InvalidBinding =>
        new(WorldQueryStatus.InvalidBinding, null);
}
