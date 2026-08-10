using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2-R2：每帧全局网格尺度合同（1/2/5 序列 + 互补交叉淡化）。
// 尺度计算不接收世界位置（API 设计保证全帧统一，禁止逐 Fragment LOD）。
public sealed class ReferenceGridScaleTests
{
    [Theory]
    // idealSpacing → Fine / Coarse（1/2/5 序列相邻两级）
    [InlineData(130, 100, 200)]
    [InlineData(340, 200, 500)]
    [InlineData(820, 500, 1000)]
    [InlineData(3100, 2000, 5000)]
    [InlineData(7400, 5000, 10000)]
    [InlineData(100, 100, 200)]
    [InlineData(200, 200, 500)]
    [InlineData(5000, 5000, 10000)]
    public void One_two_five_sequence_selects_adjacent_levels(double ideal, double expFine, double expCoarse)
    {
        var levels = ReferenceGridScale.FromIdealSpacing(ideal);
        Assert.Equal(expFine, levels.FineSpacing, 6);
        Assert.Equal(expCoarse, levels.CoarseSpacing, 6);
    }

    [Theory]
    [InlineData(130), InlineData(340), InlineData(820), InlineData(3100), InlineData(7400),
     InlineData(100), InlineData(200), InlineData(500), InlineData(1000), InlineData(5000), InlineData(9990000)]
    public void Weights_are_complementary(double ideal)
    {
        var levels = ReferenceGridScale.FromIdealSpacing(ideal);
        Assert.InRange(levels.FineWeight, 0.0, 1.0);
        Assert.InRange(levels.CoarseWeight, 0.0, 1.0);
        // 互补权重：不允许两个同时为 1（方案 8.1）。
        Assert.True(System.Math.Abs(levels.FineWeight + levels.CoarseWeight - 1.0) < 1e-9,
            $"FineWeight+CoarseWeight 必须 ≈1，实际 {levels.FineWeight + levels.CoarseWeight}");
    }

    // 边界连续：ideal 到达 coarse 时，旧 CoarseSpacing = 新 FineSpacing，权重 (0,1)→(1,0)。
    [Fact]
    public void Boundary_transition_is_continuous()
    {
        // 边界前：ideal 略小于 0.2 → fine=0.1, coarse=0.2, CoarseWeight 接近 1。
        var before = ReferenceGridScale.FromIdealSpacing(199.9);
        Assert.Equal(200, before.CoarseSpacing, 6);
        Assert.True(before.CoarseWeight > 0.99, "边界前主格权重应接近 1");
        // 边界后：ideal=0.2 → fine=0.2, coarse=0.5, FineWeight=1。
        var after = ReferenceGridScale.FromIdealSpacing(200);
        Assert.Equal(200, after.FineSpacing, 6);
        Assert.Equal(1.0, after.FineWeight, 6);
        // 同一世界间距 0.2：旧组合主格满 → 新组合细格满，无突跳。
        Assert.Equal(before.CoarseSpacing, after.FineSpacing, 6);
        Assert.False(before.FineWeight == 1.0 && before.CoarseWeight == 1.0, "不允许两个权重同时为 1");
    }

    // 钳制：ideal 超界时仍返回合法序列成员且权重互补。
    [Fact]
    public void Clamped_ideal_still_yields_valid_levels()
    {
        var low = ReferenceGridScale.FromIdealSpacing(0.0001);
        Assert.Equal(100, low.FineSpacing, 6);
        var high = ReferenceGridScale.FromIdealSpacing(10_000_000.0);
        Assert.Equal(10_000_000.0, high.FineSpacing, 6);
        Assert.Equal(1.0, high.FineWeight + high.CoarseWeight, 6);
    }

    // 全帧统一：Compute 只依赖参考世界每像素，同一输入恒同输出（无位置依赖）。
    [Fact]
    public void Same_reference_scale_yields_same_levels()
    {
        var a = ReferenceGridScale.Compute(4.0);
        var b = ReferenceGridScale.Compute(4.0);
        Assert.Equal(a, b);
        Assert.Equal(192.0, ReferenceGridScale.IdealSpacing(4.0), 6);
    }

    // 目标地图编辑视觉间距约 48 DIP。
    [Fact]
    public void Target_cell_dip_is_48()
    {
        Assert.Equal(48.0, ReferenceGridScale.TargetCellDip, 6);
    }
}
