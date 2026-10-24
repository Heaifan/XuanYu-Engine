namespace XuanYu.Core.Space;

public readonly record struct ViewportState
{
    public ViewportState(
        double logicalX,
        double logicalY,
        double logicalWidth,
        double logicalHeight,
        int physicalWidth,
        int physicalHeight,
        double dpiScale,
        long revision)
    {
        ValidateFinite(logicalX, nameof(logicalX));
        ValidateFinite(logicalY, nameof(logicalY));
        ValidatePositive(logicalWidth, nameof(logicalWidth));
        ValidatePositive(logicalHeight, nameof(logicalHeight));
        ValidatePositive(dpiScale, nameof(dpiScale));
        if (physicalWidth <= 0) throw new ArgumentOutOfRangeException(nameof(physicalWidth));
        if (physicalHeight <= 0) throw new ArgumentOutOfRangeException(nameof(physicalHeight));
        if (revision < 0) throw new ArgumentOutOfRangeException(nameof(revision));

        LogicalX = logicalX;
        LogicalY = logicalY;
        LogicalWidth = logicalWidth;
        LogicalHeight = logicalHeight;
        PhysicalWidth = physicalWidth;
        PhysicalHeight = physicalHeight;
        DpiScale = dpiScale;
        Revision = revision;
    }

    public double LogicalX { get; }

    public double LogicalY { get; }

    public double LogicalWidth { get; }

    public double LogicalHeight { get; }

    public int PhysicalWidth { get; }

    public int PhysicalHeight { get; }

    public double DpiScale { get; }

    public long Revision { get; }

    public ViewportState WithRevision(long revision) => new(
        LogicalX,
        LogicalY,
        LogicalWidth,
        LogicalHeight,
        PhysicalWidth,
        PhysicalHeight,
        DpiScale,
        revision);

    static void ValidateFinite(double value, string name)
    {
        if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(name);
    }

    static void ValidatePositive(double value, string name)
    {
        if (!double.IsFinite(value) || value <= 0.0) throw new ArgumentOutOfRangeException(name);
    }
}
