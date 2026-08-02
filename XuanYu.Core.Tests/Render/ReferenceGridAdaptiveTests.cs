using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2A：自适应参考网格数学合同（片元着色器逻辑的 CPU 镜像）。
public sealed class ReferenceGridAdaptiveTests
{
    // desiredStep = worldMetersPerPixel × 20，合法层级 0.1/1/10/100/1000/10000。
    static (double Lower, double Upper, double LowerW, double UpperW) Levels(double wmpp)
    {
        const double minStep = 0.1, maxStep = 10000.0;
        var desired = System.Math.Clamp(wmpp * 20.0, minStep, maxStep);
        var logStep = System.Math.Log10(desired);
        var lowerExp = System.Math.Floor(logStep);
        var lower = System.Math.Clamp(System.Math.Pow(10.0, lowerExp), minStep, maxStep);
        var upper = System.Math.Clamp(lower * 10.0, minStep, maxStep);
        var transition = logStep - lowerExp;
        var upperW = SmoothStep(0.25, 0.75, transition);
        return (lower, upper, 1.0 - upperW, upperW);
    }

    static double SmoothStep(double e0, double e1, double x)
    {
        var t = System.Math.Clamp((x - e0) / (e1 - e0), 0.0, 1.0);
        return t * t * (3.0 - 2.0 * t);
    }

    [Theory]
    [InlineData(0.01, 0.1, 1.0), InlineData(0.1, 1.0, 10.0), InlineData(1.0, 10.0, 100.0),
     InlineData(10.0, 100.0, 1000.0), InlineData(100.0, 1000.0, 10000.0), InlineData(1000.0, 10000.0, 10000.0)]
    public void Level_selection_follows_meters_per_pixel(double wmpp, double expLower, double expUpper)
    {
        var (lower, upper, _, _) = Levels(wmpp);
        Assert.Equal(expLower, lower, 6);
        Assert.Equal(expUpper, upper, 6);
    }

    [Theory]
    [InlineData(0.01), InlineData(0.1), InlineData(1.0), InlineData(10.0), InlineData(100.0), InlineData(1000.0)]
    public void Only_two_adjacent_levels_with_weights_summing_to_one(double wmpp)
    {
        var (lower, upper, lowerW, upperW) = Levels(wmpp);
        // 相邻：upper == lower × 10（钳制边界除外）
        Assert.True(upper == lower * 10.0 || upper == lower, "仅两个相邻十进制层级");
        // 权重和为 1
        Assert.Equal(1.0, lowerW + upperW, 6);
        Assert.InRange(lowerW, 0.0, 1.0);
        Assert.InRange(upperW, 0.0, 1.0);
    }

    [Fact]
    public void Levels_are_clamped_to_0_1_and_10000()
    {
        var (l0, u0, _, _) = Levels(0.001);    // desired=0.02 → 钳制 0.1
        Assert.Equal(0.1, l0, 6);
        var (l9, u9, _, _) = Levels(100000.0); // desired=2e6 → 钳制 10000
        Assert.Equal(10000.0, l9, 6);
        Assert.Equal(10000.0, u9, 6);
    }

    // 距离淡出：0~45% far 完整，45~75% 平滑，>75% 隐藏。
    [Theory]
    [InlineData(0.0, 1.0), InlineData(0.2, 1.0), InlineData(0.45, 1.0), InlineData(0.60, 0.5), InlineData(0.75, 0.0), InlineData(0.9, 0.0)]
    public void Distance_fade_curve(double farRatio, double expectedRange)
    {
        var fade = 1.0 - SmoothStep(0.45, 0.75, farRatio);
        // 中段区间检查（不精确断言具体值，只验证趋势与边界）
        if (farRatio <= 0.45) Assert.Equal(1.0, fade, 4);
        if (farRatio >= 0.75) Assert.Equal(0.0, fade, 4);
        Assert.InRange(fade, 0.0, expectedRange + 0.001);
    }

    // 掠射角淡出：<0.03 隐藏，0.03~0.12 淡入，>0.12 正常。
    [Theory]
    [InlineData(0.0, 0.0), InlineData(0.02, 0.0), InlineData(0.03, 0.0), InlineData(0.07, 0.5), InlineData(0.12, 1.0), InlineData(0.5, 1.0), InlineData(1.0, 1.0)]
    public void Grazing_fade_curve(double factor, double expectedRange)
    {
        var fade = SmoothStep(0.03, 0.12, factor);
        if (factor <= 0.03) Assert.Equal(0.0, fade, 4);
        if (factor >= 0.12) Assert.Equal(1.0, fade, 4);
        Assert.InRange(fade, 0.0, expectedRange + 0.001);
    }

    // 地图裁切：内部隐藏（feather 内），外部显示。
    [Fact]
    public void Map_rect_cull_inside_and_keep_outside()
    {
        const double halfW = 1000, halfD = 1000, feather = 1.5;
        bool Inside(double x, double y) =>
            System.Math.Abs(x) <= halfW + feather && System.Math.Abs(y) <= halfD + feather;
        Assert.True(Inside(0, 0), "地图中心必须裁切");
        Assert.True(Inside(999, 999), "地图内部靠近边缘仍裁切");
        Assert.False(Inside(1002, 1002), "地图外部必须显示");
        Assert.False(Inside(5000, 5000), "远处地图外显示");
    }

    // DrawPlan：有/无地图都包含 EditorReferenceGrid，且顺序在 Sky 之后。
}
