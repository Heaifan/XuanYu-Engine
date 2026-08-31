using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void Batch04(Styles styles)
    {
        var pagination = new Style(x => x.OfType<XYPagination>().Class("xyui-pagination")); pagination.Setters.Add(new Setter(Border.HeightProperty, 34d)); styles.Add(pagination);
        var current = new Style(x => x.OfType<XYIconButton>().Class("xyui-pagination-current")); Brush(current, Button.BackgroundProperty, "XY.Brush.Surface.Selected"); Brush(current, Button.ForegroundProperty, "XY.Brush.Accent.Strong"); styles.Add(current);
        var steps = new Style(x => x.OfType<XYSteps>().Class("xyui-steps")); Brush(steps, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); styles.Add(steps);
        var done = new Style(x => x.OfType<XYStepNode>().Class("xyui-step-completed")); Brush(done, Border.BackgroundProperty, "XY.Brush.Accent.Default"); styles.Add(done);
        var active = new Style(x => x.OfType<XYStepNode>().Class("xyui-step-current")); Brush(active, Border.BackgroundProperty, "XY.Brush.Surface.Selected"); Brush(active, Border.BorderBrushProperty, "XY.Brush.Accent.Default"); active.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(2))); styles.Add(active);
        var pending = new Style(x => x.OfType<XYStepNode>().Class("xyui-step-pending")); Brush(pending, Border.BorderBrushProperty, "XY.Brush.Border.Color.Subtle"); styles.Add(pending);
        var toolbar = new Style(x => x.OfType<XYToolbar>().Class("xyui-toolbar")); Brush(toolbar, Border.BackgroundProperty, "XY.Brush.Surface.Toolbar"); toolbar.Setters.Add(new Setter(Border.HeightProperty, 38d)); styles.Add(toolbar);
        var tool = new Style(x => x.OfType<XYToolbarTool>().Class("xyui-toolbar-tool")); tool.Setters.Add(new Setter(Control.HeightProperty, 32d)); styles.Add(tool);
        var group = new Style(x => x.OfType<XYToolGroup>().Class("xyui-tool-group")); group.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(4, 0))); group.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4))); styles.Add(group);
        var collapsed = new Style(x => x.OfType<XYToolGroup>().Class("xyui-tool-group-collapsed")); Brush(collapsed, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(collapsed);
    }
}
