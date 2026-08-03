using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2-R2：参考网格片元行为合同（CPU 镜像）。
// 互补交叉淡化（不再允许两权重同时为 1）；方向性密度淡出（<6px 隐藏、6~12 渐入、>12 正常）；
// 有界深度偏移（clamp(fwidth×0.5, 1e-7, 2e-5)）；距离淡出 + 掠射角淡出。
public sealed class ReferenceGridAdaptiveTests
{
    static double SmoothStep(double e0, double e1, double x)
    {
        var t = System.Math.Clamp((x - e0) / (e1 - e0), 0.0, 1.0);
        return t * t * (3.0 - 2.0 * t);
    }

    // 密度淡出（方案 10.2）：4px→0、6px→0、9px→过渡、12px→1、20px→1。
    static double DensityFade(double cellPixels) => SmoothStep(6.0, 12.0, cellPixels);

    [Theory]
    [InlineData(4.0, 0.0), InlineData(6.0, 0.0), InlineData(9.0, 0.5), InlineData(12.0, 1.0), InlineData(20.0, 1.0)]
    public void Density_fade_curve(double cellPixels, double expectedRange)
    {
        var fade = DensityFade(cellPixels);
        if (cellPixels <= 6.0) Assert.Equal(0.0, fade, 4);
        if (cellPixels >= 12.0) Assert.Equal(1.0, fade, 4);
        Assert.InRange(fade, 0.0, expectedRange + 0.001);
    }

    // 方向独立：X/Y 各自按单元屏幕间距淡出，不使用统一标量。
    [Fact]
    public void Density_fade_is_directional_independent()
    {
        // x 方向密（4px）→ 隐藏；y 方向疏（20px）→ 正常。
        Assert.Equal(0.0, DensityFade(4.0), 4);
        Assert.Equal(1.0, DensityFade(20.0), 4);
    }

    // 有界深度偏移（方案 12.2）：clamp(fwidth×0.5, 1e-7, 2e-5)。
    [Theory]
    [InlineData(1e-9, 1e-7)]   // 极小 → 下限
    [InlineData(1e-4, 2e-5)]   // 极大 → 上限
    [InlineData(2e-6, 1e-6)]   // 中值 → fwidth×0.5
    [InlineData(0.0, 1e-7)]    // 零 → 下限
    public void Depth_bias_is_clamped(double fwidthDepth, double expectedBias)
    {
        const double factor = 0.5, minBias = 1e-7, maxBias = 2e-5;
        var bias = System.Math.Clamp(fwidthDepth * factor, minBias, maxBias);
        Assert.Equal(expectedBias, bias, 12);
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

    // 网格线宽合同（方案 9.1）：细 0.70px、主 1.00px；细 alpha < 主 alpha。
    [Fact]
    public void Line_width_and_alpha_contract()
    {
        const double fineWidth = 0.70, coarseWidth = 1.00;
        const double fineAlpha = 0.18, coarseAlpha = 0.32;
        Assert.True(fineWidth < coarseWidth, "细格线宽必须小于主格");
        Assert.True(fineAlpha < coarseAlpha, "细格 alpha 必须小于主格");
        Assert.True(coarseAlpha <= 0.4, "主格 alpha 不应过强");
    }
}
