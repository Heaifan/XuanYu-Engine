namespace XuanYu.Render.Abstractions;

// GRID-RW-1：一帧只有一个网格尺度与相机吸附锚点，禁止把 LOD 下放到 Fragment。
// GRID-RW-1-CORR2：Step 选择使用保守尺度 max(X,Y)——斜视下 X/Y 各向异性（如 2/30 m/DIP），
// 只要任一方向过密就升级网格；公共 ViewportMetricScale.MetersPerDip（min）继续服务比例尺，不受影响。
public readonly record struct ReferenceGridFrameState(
    double StepMeters, double AnchorX, double AnchorY, double BaseHeightMeters)
{
    public const double MinStepMeters = 100.0;
    public const double MaxStepMeters = 10_000_000.0;
    public const double MinCellDip = 10.0;
    public const double MaxCellDip = 140.0;

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
        while (step / metersPerDip < MinCellDip && step < MaxStepMeters) step *= 10.0;
        while (step / metersPerDip > MaxCellDip && step > MinStepMeters) step /= 10.0;
        return step;
    }

    static double Snap(double value, double step) =>
        System.Math.Round(value / step, MidpointRounding.AwayFromZero) * step;

    static bool IsValidStep(double step) =>
        double.IsFinite(step) && step >= MinStepMeters && step <= MaxStepMeters;
}
