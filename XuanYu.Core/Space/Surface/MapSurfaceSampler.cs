namespace XuanYu.Core.Map;

// MAP-A-R1-D3：唯一地表采样源。
// World 高度查询与 D4 Render 网格生成必须共用本采样器，禁止第二套起伏公式。
// 同一 (kind, 参数, x, y) 永远返回相同高度：纯算术，无随机。
public static class MapSurfaceSampler
{
    public static double SampleHeight(
        MapSurfaceKind kind,
        double baseHeightMeters,
        double amplitudeMeters,
        double wavelengthMeters,
        int seed,
        double x,
        double y)
    {
        return kind switch
        {
            MapSurfaceKind.Flat => baseHeightMeters,
            MapSurfaceKind.GentleHillsV1 => GentleHills(baseHeightMeters, amplitudeMeters, wavelengthMeters, seed, x, y),
            _ => baseHeightMeters
        };
    }

    // 确定性缓丘：两个正交正弦叠加，相位由 seed 固定派生。
    // 输出范围 [base - amplitude, base + amplitude]。
    static double GentleHills(
        double baseHeight,
        double amplitude,
        double wavelength,
        int seed,
        double x,
        double y)
    {
        const double tau = System.Math.PI * 2.0;
        var phaseX = seed * 0.6180339887498949; // 黄金比例，固定相位常数
        var phaseY = seed * 0.3819660112501051; // 黄金比例平方倒数，固定相位常数
        var wave = tau / wavelength;
        var hx = System.Math.Sin(x * wave + phaseX);
        var hy = System.Math.Cos(y * wave + phaseY);
        return baseHeight + amplitude * (0.5 * hx + 0.5 * hy);
    }
}
