namespace FluidWarfare.Render.Selection.Ground;

/// <summary>
/// 水平地面平面定义（Z-Up）。
/// 地面法线为 +Z，使用 ElevationZ 表示高度。
/// 默认地面 Z = 0。
/// </summary>
public sealed record SceneGroundPlane(double ElevationZ)
{
    /// <summary>默认地面（Z = 0）。</summary>
    public static readonly SceneGroundPlane Default = new(0);

    /// <summary>地面法线（+Z 方向）。</summary>
    public const double NormalX = 0;
    public const double NormalY = 0;
    public const double NormalZ = 1;
}
