using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Layout;

public abstract class XyuiSemanticStack : StackPanel
{
    protected XyuiSemanticStack()
    {
        PropertyChanged += (_, args) =>
        {
            if (args.Property == XyuiDensityScope.ModeProperty) ApplyDensity();
        };
        ApplyDensity();
    }

    protected abstract double Gap(XyuiDensitySemanticMetrics metrics);

    void ApplyDensity()
    {
        if (XyuiDensityScope.TryGetSemanticMetrics(this, out var metrics))
            Spacing = Gap(metrics);
    }
}
