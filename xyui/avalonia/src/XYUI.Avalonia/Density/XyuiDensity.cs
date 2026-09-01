using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Density;

public enum XyuiDensityMode { Compact, Comfortable, Touch }

public enum XyuiDensityPolicy { Auto, ManualLock, Hysteresis }

// Density 是离散档位；策略独立表达切换方式，不与档位混合。
public static class XyuiDensity
{
    public static ResourceDictionary CreateResources()
    {
        return new ResourceDictionary
        {
            ["XY.Density.Compact.TreeRow"] = XyuiSizeTokens.TreeRow,
            ["XY.Density.Compact.Toolbar"] = XyuiSizeTokens.Toolbar,
            ["XY.Density.Compact.Input"] = XyuiSizeTokens.Input,
            ["XY.Density.Compact.Gap"] = XyuiSpatialTokens.FieldRowGap,
            ["XY.Density.Compact.SectionGap"] = XyuiSpatialTokens.SectionGap,
            ["XY.Density.Comfortable.TreeRow"] = 32d,
            ["XY.Density.Comfortable.Toolbar"] = 34d,
            ["XY.Density.Comfortable.Input"] = 36d,
            ["XY.Density.Comfortable.Gap"] = 8d,
            ["XY.Density.Comfortable.SectionGap"] = 16d,
        };
    }

    public static bool TryGetMetrics(XyuiDensityMode mode, out XyuiDensityMetrics metrics)
    {
        if (mode == XyuiDensityMode.Touch)
        {
            metrics = default;
            return false;
        }

        metrics = mode == XyuiDensityMode.Compact
            ? new(XyuiSizeTokens.ControlS, XyuiSizeTokens.Toolbar, XyuiSizeTokens.Input,
                XyuiSizeTokens.IconM, XyuiSpatialTokens.FieldRowGap, XyuiSpatialTokens.PanelPadding)
            : new(32d, 34d, 36d, XyuiSizeTokens.IconM, 8d, XyuiSpatialTokens.PanelPadding);
        return true;
    }
}

public readonly record struct XyuiDensityMetrics(
    double ControlSize, double ToolbarSize, double InputSize, double IconSize,
    double Gap, double Padding);
