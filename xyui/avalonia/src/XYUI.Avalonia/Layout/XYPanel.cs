using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Layout;

public sealed class XYPanel : Border
{
    public XYPanel()
    {
        PropertyChanged += (_, args) =>
        {
            if (args.Property == XyuiDensityScope.ModeProperty) ApplyDensity();
        };
        ApplyDensity();
    }

    void ApplyDensity()
    {
        if (XyuiDensityScope.TryGetSemanticMetrics(this, out var metrics))
            Padding = new Thickness(metrics.PanelPadding);
    }
}
