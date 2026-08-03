using System;

namespace XuanYu.Render.Abstractions;

// MAP-A-R1-D5-R1-F2-R2：每帧统一参考网格尺度（1/2/5 十进制序列 + 互补交叉淡化）。
// 同一帧所有 Fragment 使用同一组 Fine/Coarse/权重——禁止逐 Fragment 选择 LOD。
// 不持有相机、不调用 Vulkan、不保存地图数据；纯数学职责。
public readonly record struct ReferenceGridLevels(
    double FineSpacing,
    double CoarseSpacing,
    double FineWeight,
    double CoarseWeight);

public static class ReferenceGridScale
{
    public const double TargetCellPixels = 48.0;
    public const double MinSpacing = 0.01;
    public const double MaxSpacing = 1000.0;

    // idealSpacing → 1/2/5 序列相邻两级 + 对数域互补权重。
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

    // 参考世界每像素 → 理想间距（目标 48px/格）。
    public static double IdealSpacing(double referenceWorldPerPixel)
    {
        return referenceWorldPerPixel * TargetCellPixels;
    }

    // 便捷入口：CPU 每帧调用一次。
    public static ReferenceGridLevels Compute(double referenceWorldPerPixel)
    {
        return FromIdealSpacing(IdealSpacing(referenceWorldPerPixel));
    }

    // 序列中最大的 ≤ ideal 成员（1/2/5 十进制：...0.01, 0.02, 0.05, 0.1...）。
    static double PickFine(double ideal)
    {
        var decade = Math.Pow(10.0, Math.Floor(Math.Log10(ideal)));
        if (decade * 5.0 <= ideal) return decade * 5.0;
        if (decade * 2.0 <= ideal) return decade * 2.0;
        return decade;
    }

    // 1/2/5 序列中 fine 的下一个成员。
    static double NextStep(double fine)
    {
        var decade = Math.Pow(10.0, Math.Floor(Math.Log10(fine)));
        if (fine >= decade * 5.0) return Math.Min(decade * 10.0, MaxSpacing);
        if (fine >= decade * 2.0) return decade * 5.0;
        return decade * 2.0;
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
