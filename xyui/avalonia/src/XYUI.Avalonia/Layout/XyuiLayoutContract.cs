namespace XYUI.Avalonia.Layout;

using XYUI.Avalonia.Density;

public enum XyuiLayoutRecipe
{
    Toolbar, Form, Inspector, Menu, Popup, Dialog, PropertyGrid,
}

public sealed record XyuiLayoutContract(
    XyuiLayoutRecipe Recipe,
    bool ComponentOwnsPadding,
    bool ParentOwnsSiblingGap,
    bool MarginIsSiblingLayoutTool);

public static class XyuiLayoutContracts
{
    public static XyuiLayoutContract For(XyuiLayoutRecipe recipe) =>
        new(recipe, ComponentOwnsPadding: true, ParentOwnsSiblingGap: true,
            MarginIsSiblingLayoutTool: false);

    public static bool TryMetrics(XyuiLayoutRecipe recipe, XyuiDensityMode mode,
        out XyuiCompositionMetrics metrics)
    {
        if (!XyuiDensity.TryGetMetrics(mode, out var density) ||
            !XyuiDensity.TryGetSemanticMetrics(mode, out var semantic))
        {
            metrics = default;
            return false;
        }

        metrics = new(recipe, density.ControlSize, density.ToolbarSize, density.InputSize,
            density.IconSize, semantic.FieldGap, semantic.PanelPadding,
            semantic.ToolItemGap, semantic.ToolGroupGap, semantic.IconTextGap,
            semantic.SectionGap);
        return true;
    }
}

public readonly record struct XyuiCompositionMetrics(
    XyuiLayoutRecipe Recipe, double ControlSize, double ToolbarSize, double InputSize,
    double IconSize, double Gap, double Padding, double ToolItemGap,
    double ToolGroupGap, double IconTextGap, double SectionGap);
