using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2：自适应参考网格数学合同（片元着色器逻辑的 CPU 镜像）。
// desiredStep = wmpp×36（目标 36px/格，24~48）；细格基础 α0.20 + 主格加深 α0.18；
// 跨级同组线 alpha 差 ≤ 0.02（主格 0.18 → 细格 0.20，透明度连续）。
public sealed class ReferenceGridAdaptiveTests
{
    static (double Lower, double Upper, double FineW, double MajorW) Levels(double wmpp)
    {
        const double minStep = 0.1, maxStep = 10000.0;
        var desired = System.Math.Clamp(wmpp * 36.0, minStep, maxStep);
        var logStep = System.Math.Log10(desired);
        var lowerExp = System.Math.Floor(logStep);
        var lower = System.Math.Clamp(System.Math.Pow(10.0, lowerExp), minStep, maxStep);
        var upper = System.Math.Clamp(lower * 10.0, minStep, maxStep);
        var phase = logStep - lowerExp;
        var fineW = 1.0 - SmoothStep(0.5, 1.0, phase);
        var majorW = SmoothStep(0.0, 0.5, phase);
        return (lower, upper, fineW, majorW);
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
    public void Two_adjacent_levels_with_weights_in_range(double wmpp)
    {
        var (lower, upper, fineW, majorW) = Levels(wmpp);
        Assert.True(upper == lower * 10.0 || upper == lower, "仅两个相邻十进制层级");
        Assert.InRange(fineW, 0.0, 1.0);
        Assert.InRange(majorW, 0.0, 1.0);
    }
    [Fact]
    public void Levels_are_clamped_to_0_1_and_10000()
    {
        var (l0, u0, _, _) = Levels(0.001);    // desired=0.036 → 钳制 0.1
        Assert.Equal(0.1, l0, 6);
        var (l9, u9, _, _) = Levels(100000.0); // desired=3.6e6 → 钳制 10000
        Assert.Equal(10000.0, l9, 6);
        Assert.Equal(10000.0, u9, 6);
    }
    [Fact]
    public void Cross_level_alpha_continuity_within_two_percent()
    {
        const double minorAlpha = 0.20, majorExtra = 0.18;
        // 层级 n 末端：desired 接近 10^(n+1)（phase→1），10^(n+1) 作为主格线。
        var (_, upperEnd, fineWEnd, majorWEnd) = Levels(2.7);      // desired≈97.2, phase≈0.988
        var alphaAsMajor = minorAlpha * fineWEnd + majorExtra * majorWEnd;
        // 层级 n+1 起始：desired = 10^(n+1)（phase=0），10^(n+1) 作为细格线。
        var (lowerStart, _, fineWStart, _) = Levels(100.0 / 36.0); // desired=100, phase=0
        Assert.Equal(100.0, upperEnd, 6);
        Assert.Equal(100.0, lowerStart, 6);
        var alphaAsMinor = minorAlpha * fineWStart;
        Assert.True(System.Math.Abs(alphaAsMajor - alphaAsMinor) <= 0.02,
            $"跨级 alpha 差 {System.Math.Abs(alphaAsMajor - alphaAsMinor):F3} 必须 ≤ 0.02");
    }
    // 交叉淡化中心（phase=0.5）：细格满、主格加深满 → 主格线 alpha 峰值 0.38。
    [Fact]
    public void Major_line_alpha_peaks_at_crossfade_center()
    {
        const double minorAlpha = 0.20, majorExtra = 0.18;
        var (_, _, fineW, majorW) = Levels(System.Math.Pow(10.0, 1.5) / 36.0); // desired=31.62, phase=0.5
        Assert.Equal(1.0, fineW, 6);
        Assert.Equal(1.0, majorW, 6);
        Assert.Equal(0.38, minorAlpha * fineW + majorExtra * majorW, 6);
    }
    // 距离淡出：0~45% far 完整，45~75% 平滑，>75% 隐藏。
    [Theory]
    [InlineData(0.0, 1.0), InlineData(0.2, 1.0), InlineData(0.45, 1.0), InlineData(0.60, 0.5), InlineData(0.75, 0.0), InlineData(0.9, 0.0)]
    public void Distance_fade_curve(double farRatio, double expectedRange)
    {
        var fade = 1.0 - SmoothStep(0.45, 0.75, farRatio);
        if (farRatio <= 0.45) Assert.Equal(1.0, fade, 4);
        if (farRatio >= 0.75) Assert.Equal(0.0, fade, 4);
        Assert.InRange(fade, 0.0, expectedRange + 0.001);
    }
    // 掠射角淡出：<0.015 隐藏，0.015~0.080 淡入，>0.080 正常。
    [Theory]
    [InlineData(0.0, 0.0), InlineData(0.01, 0.0), InlineData(0.015, 0.0), InlineData(0.04, 0.5), InlineData(0.08, 1.0), InlineData(0.5, 1.0), InlineData(1.0, 1.0)]
    public void Grazing_fade_curve(double factor, double expectedRange)
    {
        var fade = SmoothStep(0.015, 0.080, factor);
        if (factor <= 0.015) Assert.Equal(0.0, fade, 4);
        if (factor >= 0.08) Assert.Equal(1.0, fade, 4);
        Assert.InRange(fade, 0.0, expectedRange + 0.001);
    }
}
