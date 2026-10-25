using System;

namespace XuanYu.Render.Abstractions;

// MAP-A-R3-D2-F1-V3：地图编辑器参考网格（100m 起步 + 2 倍嵌套序列）。
// 同一帧所有 Fragment 使用同一组 Fine/Coarse/权重——禁止逐 Fragment 选择 LOD。
// 不持有相机、不调用 Vulkan、不保存地图数据；纯数学职责。
public readonly record struct ReferenceGridLevels(
    double FineSpacing,
    double CoarseSpacing,
    double FineWeight,
    double CoarseWeight);

public static class ReferenceGridScale
{
    public const double TargetCellDip = 48.0;
    public const double MinSpacing = 100.0;
    public const double MaxSpacing = 1_000_000_000_000.0;

    // idealSpacing → 2 倍嵌套序列相邻两级 + 对数域互补权重。
    // 边界连续：ideal 到达 coarse 时，旧 CoarseSpacing = 新 FineSpacing，
    // 权重从 (0,1) 无缝切到 (1,0)，同一世界间距不突跳。
    public static ReferenceGridLevels FromIdealSpacing(double idealSpacing)
    {
        var ideal = Math.Clamp(idealSpacing, MinSpacing, MaxSpacing);
        var fine = PickFine(ideal);
        var coarse = NextStep(fine);
        var blend = SmoothStep(0.0, 1.0, Phase(ideal, fine, coarse));
        return new ReferenceGridLevels(fine, coarse, 1.0 - blend, blend);
    }

    // 参考世界每 DIP → 理想间距（目标 48 DIP/格）。
    public static double IdealSpacing(double referenceWorldPerDip)
    {
        return referenceWorldPerDip * TargetCellDip;
    }

    // 便捷入口：CPU 每帧调用一次。
    public static ReferenceGridLevels Compute(double referenceWorldPerDip)
    {
        return FromIdealSpacing(IdealSpacing(referenceWorldPerDip));
    }

    public static ReferenceGridLevels Compute(ViewportMetricScale metric)
    {
        return Compute(Math.Max(metric.MetersPerDipX, metric.MetersPerDipY));
    }

    // Grid 只允许 2 倍层级，保证 coarse 线是 fine 线的严格子集。
    static double PickFine(double ideal)
    {
        if (ideal >= MaxSpacing / 2.0) return MaxSpacing;
        var exponent = Math.Floor(Math.Log(ideal / MinSpacing, 2.0));
        return Math.Min(MinSpacing * Math.Pow(2.0, exponent), MaxSpacing);
    }

    // 末级保持自身，禁止因不存在 NextLOD 把整体 alpha 淡出为 0。
    static double NextStep(double fine)
    {
        return fine >= MaxSpacing / 2.0 ? fine : Math.Min(fine * 2.0, MaxSpacing);
    }

    public static double LargestNiceSpacingAtMost(double value)
    {
        if (!double.IsFinite(value) || value <= 0.0) return 0.0;
        var exponent = Math.Floor(Math.Log(value / MinSpacing, 2.0));
        return Math.Min(MinSpacing * Math.Pow(2.0, exponent), MaxSpacing);
    }

    public static double NextNiceSpacing(double spacing)
    {
        if (!double.IsFinite(spacing) || spacing <= 0.0) return 0.0;
        return spacing >= MaxSpacing / 2.0 ? MaxSpacing : spacing * 2.0;
    }

    // 对数域相位：fine 时 0，coarse 时 1。
    static double Phase(double ideal, double fine, double coarse)
    {
        if (coarse <= fine) return 0.0;
        return (Math.Log10(ideal) - Math.Log10(fine))
             / (Math.Log10(coarse) - Math.Log10(fine));
    }

    static double SmoothStep(double e0, double e1, double x)
    {
        var t = Math.Clamp((x - e0) / (e1 - e0), 0.0, 1.0);
        return t * t * (3.0 - 2.0 * t);
    }
}
