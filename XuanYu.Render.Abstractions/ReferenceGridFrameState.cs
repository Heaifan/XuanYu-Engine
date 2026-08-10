namespace XuanYu.Render.Abstractions;

// GRID-RW-2B：一帧只有一个 World Grid Step，禁止把 LOD 下放到 Fragment。
// Step 使用保守尺度 max(X,Y)，维持 24~80 DIP 的宽回滞，并按 1/2/5 序列切换。
public readonly record struct ReferenceGridFrameState(
    double StepMeters, double AnchorX, double AnchorY, double BaseHeightMeters)
{
    public const double MinStepMeters = 100.0;
    public const double MaxStepMeters = 10_000_000.0;
    public const double MinCellDip = 24.0;
    public const double MaxCellDip = 80.0;

    public static ReferenceGridFrameState Create(
        ViewportMetricScale metric, double cameraX, double cameraY,
        double baseHeightMeters, ReferenceGridFrameState previous)
    {
        var conservative = Math.Max(metric.MetersPerDipX, metric.MetersPerDipY);
        var step = SelectStep(conservative, previous.StepMeters);
        return new(step, Snap(cameraX, step), Snap(cameraY, step), baseHeightMeters);
    }

    static double SelectStep(double metersPerDip, double previousStep)
    {
        var step = IsValidStep(previousStep) ? previousStep : MinStepMeters;
        if (!double.IsFinite(metersPerDip) || metersPerDip <= 0.0) return step;
        while (step / metersPerDip < MinCellDip && step < MaxStepMeters) step = NextStep(step);
        while (step / metersPerDip > MaxCellDip && step > MinStepMeters) step = PreviousStep(step);
        return step;
    }

    static double NextStep(double step)
    {
        var scale = Math.Pow(10.0, Math.Floor(Math.Log10(step)));
        var significant = step / scale;
        return significant < 1.5 ? 2.0 * scale : significant < 3.5 ? 5.0 * scale : 10.0 * scale;
    }

    static double PreviousStep(double step)
    {
        var scale = Math.Pow(10.0, Math.Floor(Math.Log10(step)));
        var significant = step / scale;
        return significant > 3.5 ? 2.0 * scale : significant > 1.5 ? scale : 0.5 * scale;
    }

    static double Snap(double value, double step) =>
        System.Math.Round(value / step, MidpointRounding.AwayFromZero) * step;

    static bool IsValidStep(double step) =>
        double.IsFinite(step) && step >= MinStepMeters && step <= MaxStepMeters;
}
