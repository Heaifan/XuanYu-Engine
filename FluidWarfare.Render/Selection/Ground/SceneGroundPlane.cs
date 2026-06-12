namespace FluidWarfare.Render.Selection.Ground;

/// <summary>
/// 水平地面平面定义。
/// 当前使用 Y=0 平面，Height 明确封装以便后续替换为地形高度查询。
/// </summary>
public sealed record SceneGroundPlane(double Height)
{
    public static readonly SceneGroundPlane Default = new(0);

    /// <summary>地面法线（+Y 方向）。</summary>
    public const double NormalX = 0;
    public const double NormalY = 1;
    public const double NormalZ = 0;
}
