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
        var resources = new ResourceDictionary
        {
            ["XY.Density.Compact.TreeRow"] = XyuiSizeTokens.TreeRow,
            ["XY.Density.Compact.Toolbar"] = XyuiSizeTokens.Toolbar,
            ["XY.Density.Compact.Input"] = XyuiSizeTokens.Input,
            ["XY.Density.Compact.Gap"] = 6d,
            ["XY.Density.Compact.SectionGap"] = 12d,
            ["XY.Density.Comfortable.TreeRow"] = 32d,
            ["XY.Density.Comfortable.Toolbar"] = 34d,
            ["XY.Density.Comfortable.Input"] = 36d,
            ["XY.Density.Comfortable.Gap"] = 8d,
            ["XY.Density.Comfortable.SectionGap"] = 16d,
        };
        foreach (var mode in Enum.GetValues<XyuiDensityMode>())
            foreach (var item in CreateSemanticResources(mode)) resources[item.Key] = item.Value;
        return resources;
    }
    public static ResourceDictionary CreateSemanticResources(XyuiDensityMode mode)
    {
        var values = mode switch
        {
            XyuiDensityMode.Compact => (2d, 6d, 4d, 6d, 12d, 12d),
            XyuiDensityMode.Comfortable => (4d, 8d, 4d, 8d, 16d, 16d),
            XyuiDensityMode.Touch => (6d, 12d, 6d, 12d, 20d, 20d),
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
        return new ResourceDictionary
        {
            [$"XY.Density.{mode}.Gap.ToolItem"] = values.Item1,
            [$"XY.Density.{mode}.Gap.ToolGroup"] = values.Item2,
            [$"XY.Density.{mode}.Gap.IconText"] = values.Item3,
            [$"XY.Density.{mode}.Gap.Field"] = values.Item4,
            [$"XY.Density.{mode}.Gap.Section"] = values.Item5,
            [$"XY.Density.{mode}.Padding.Panel"] = values.Item6,
        };
    }
    public static bool TryGetSemanticMetrics(XyuiDensityMode mode,
        out XyuiDensitySemanticMetrics metrics)
    {
        var resources = CreateSemanticResources(mode);
        metrics = new(
            (double)resources[$"XY.Density.{mode}.Gap.ToolItem"]!,
            (double)resources[$"XY.Density.{mode}.Gap.ToolGroup"]!,
            (double)resources[$"XY.Density.{mode}.Gap.IconText"]!,
            (double)resources[$"XY.Density.{mode}.Gap.Field"]!,
            (double)resources[$"XY.Density.{mode}.Gap.Section"]!,
            (double)resources[$"XY.Density.{mode}.Padding.Panel"]!);
        return true;
    }
    public static ResourceDictionary CreateResolvedSemanticResources(XyuiDensityMode mode)
    {
        if (!TryGetSemanticMetrics(mode, out var metrics)) return new();
        return new ResourceDictionary
        {
            ["XY.Gap.ToolItem"] = metrics.ToolItemGap,
            ["XY.Gap.ToolGroup"] = metrics.ToolGroupGap,
            ["XY.Gap.IconText"] = metrics.IconTextGap,
            ["XY.Gap.Field"] = metrics.FieldGap,
            ["XY.Gap.Section"] = metrics.SectionGap,
            ["XY.Padding.Panel"] = metrics.PanelPadding,
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
                XyuiSizeTokens.IconM, 6d, 12d)
            : new(32d, 34d, 36d, XyuiSizeTokens.IconM, 8d, 16d);
        return true;
    }
}

public readonly record struct XyuiDensityMetrics(double ControlSize, double ToolbarSize,
    double InputSize, double IconSize, double Gap, double Padding);
public readonly record struct XyuiDensitySemanticMetrics(double ToolItemGap, double ToolGroupGap,
    double IconTextGap, double FieldGap, double SectionGap, double PanelPadding);
