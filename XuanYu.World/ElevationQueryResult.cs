namespace XuanYu.World;

public readonly record struct ElevationQueryResult(
    WorldQueryStatus Status, double? Value)
{
    public bool IsValid => Status == WorldQueryStatus.Valid;

    public double ElevationMeters => IsValid
        ? Value!.Value
        : throw new InvalidOperationException("无效查询没有高程值。");

    public static ElevationQueryResult Valid(double meters) =>
        new(WorldQueryStatus.Valid, meters);

    public static ElevationQueryResult NoData =>
        new(WorldQueryStatus.NoData, null);

    public static ElevationQueryResult OutOfBounds =>
        new(WorldQueryStatus.OutOfBounds, null);

    public static ElevationQueryResult NoTerrain =>
        new(WorldQueryStatus.NoTerrain, null);

    public static ElevationQueryResult InvalidBinding =>
        new(WorldQueryStatus.InvalidBinding, null);
}
