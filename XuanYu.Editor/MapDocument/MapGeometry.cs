namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图尺寸（米）。width 对应世界 X，depth 对应世界 Y，Z-Up 下高度沿 Z。
public sealed record MapSize(double Width, double Depth);

// MAP-A-R1-D2：通用三维数值（坐标原点 / 太阳方向共用）。
public sealed record MapVector3(double X, double Y, double Z)
{
    public bool IsZero => X == 0.0 && Y == 0.0 && Z == 0.0;
}

// MAP-A-R1-D2：坐标系统。R1 固定 meter + Z-Up + 零原点，其余一律拒绝。
public sealed record MapCoordinateSystem(
    string Unit,
    string UpAxis,
    MapVector3 Origin)
{
    public static MapCoordinateSystem ZUpMeter { get; } = new("meter", "Z", new MapVector3(0, 0, 0));
}
