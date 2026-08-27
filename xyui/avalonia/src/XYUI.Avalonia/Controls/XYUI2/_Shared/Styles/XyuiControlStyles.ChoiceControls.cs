using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Spatial;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void AddChoiceControls(Styles styles)
    {
        AddCheckbox(styles); AddRadio(styles); AddSwitch(styles);
    }

    static void AddCheckbox(Styles styles)
    {
        var root = new Style(x => x.OfType<XYCheckbox>().Class("xyui-checkbox"));
        root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYCheckbox.CreateTemplate()));
        Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI");
        Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); Set(root, TemplatedControl.FontWeightProperty, "XY.FontWeight.Normal");
        root.Setters.Add(new Setter(Control.MinHeightProperty, 22d)); styles.Add(root);
        var box = Visual<XYCheckbox, Border>(styles, "xyui-checkbox", "xyui-checkbox-box");
        Set(box, Border.WidthProperty, "XY.Size.Checkbox"); Set(box, Border.HeightProperty, "XY.Size.Checkbox");
        Set(box, Border.BackgroundProperty, "XY.Brush.Surface.Input"); Set(box, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        Set(box, Border.CornerRadiusProperty, "XY.Checkbox.Radius"); box.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); styles.Add(box);
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":checked", Border.BackgroundProperty, "XY.Brush.Surface.Selected");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":checked", Border.BorderBrushProperty, "XY.Brush.Border.Color.Selected");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":indeterminate", Border.BackgroundProperty, "XY.Brush.Surface.Selected");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":indeterminate", Border.BorderBrushProperty, "XY.Brush.Border.Color.Selected");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":pointerover", Border.BackgroundProperty, "XY.Brush.State.Color.Hover");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":checked:pointerover", Border.BackgroundProperty, "XY.Brush.Surface.Selected");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":indeterminate:pointerover", Border.BackgroundProperty, "XY.Brush.Surface.Selected");
        VisualState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-box", ":focus", Border.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
        MarkBase(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-check"); MarkBase(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-mixed");
        MarkState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-check", ":checked", VectorPath.StrokeProperty, "XY.Brush.Accent.Strong");
        MarkState(styles, typeof(XYCheckbox), "xyui-checkbox", "xyui-checkbox-mixed", ":indeterminate", Border.BackgroundProperty, "XY.Brush.Accent.Strong");
        Disabled(styles, typeof(XYCheckbox), "xyui-checkbox");
    }

    static void AddRadio(Styles styles)
    {
        var root = new Style(x => x.OfType<XYRadioButton>().Class("xyui-radio-button"));
        root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYRadioButton.CreateTemplate()));
        Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); styles.Add(root);
        var host = Visual<XYRadioButton, Grid>(styles, "xyui-radio-button", "xyui-radio-host"); Set(host, Control.WidthProperty, "XY.Radio.HaloSize"); Set(host, Control.HeightProperty, "XY.Radio.HaloSize"); styles.Add(host);
        var halo = Visual<XYRadioButton, Ellipse>(styles, "xyui-radio-button", "xyui-radio-halo"); Set(halo, Control.WidthProperty, "XY.Radio.HaloSize"); Set(halo, Control.HeightProperty, "XY.Radio.HaloSize"); styles.Add(halo);
        VisualState(styles, typeof(XYRadioButton), "xyui-radio-button", "xyui-radio-halo", ":checked", Shape.FillProperty, "XY.Brush.Accent.Soft");
        VisualState(styles, typeof(XYRadioButton), "xyui-radio-button", "xyui-radio-halo", ":pointerover", Shape.FillProperty, "XY.Brush.State.Color.Hover");
        var circle = Visual<XYRadioButton, Ellipse>(styles, "xyui-radio-button", "xyui-radio-circle"); Set(circle, Control.WidthProperty, "XY.Size.Radio"); Set(circle, Control.HeightProperty, "XY.Size.Radio"); Set(circle, Shape.FillProperty, "XY.Brush.Surface.Input"); Set(circle, Shape.StrokeProperty, "XY.Brush.Border.Color.Default"); circle.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 1d)); styles.Add(circle);
        VisualState(styles, typeof(XYRadioButton), "xyui-radio-button", "xyui-radio-circle", ":checked", Shape.StrokeProperty, "XY.Brush.Border.Color.Selected"); VisualState(styles, typeof(XYRadioButton), "xyui-radio-button", "xyui-radio-circle", ":focus", Shape.StrokeProperty, "XY.Brush.Border.Color.Focus");
        MarkBase(styles, typeof(XYRadioButton), "xyui-radio-button", "xyui-radio-dot"); MarkState(styles, typeof(XYRadioButton), "xyui-radio-button", "xyui-radio-dot", ":checked", Shape.FillProperty, "XY.Brush.Accent.Strong"); Disabled(styles, typeof(XYRadioButton), "xyui-radio-button");
    }

    static void AddSwitch(Styles styles)
    {
        var root = new Style(x => x.OfType<XYSwitch>().Class("xyui-switch")); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYSwitch.CreateTemplate()));
        Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); styles.Add(root);
        var track = Visual<XYSwitch, Border>(styles, "xyui-switch", "xyui-switch-track"); Set(track, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); Set(track, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); Set(track, Border.CornerRadiusProperty, "XY.Radius.Full"); track.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); styles.Add(track);
        var host = new Style(x => x.OfType<XYSwitch>().Class("xyui-switch").Descendant().OfType<Grid>().Class("xyui-switch-host")); Set(host, Control.WidthProperty, "XY.Size.Switch.Width"); Set(host, Control.HeightProperty, "XY.Size.Switch.Height"); styles.Add(host);
        Set(track, Border.WidthProperty, "XY.Size.Switch.Width"); Set(track, Border.HeightProperty, "XY.Size.Switch.Height");
        VisualState(styles, typeof(XYSwitch), "xyui-switch", "xyui-switch-track", ":checked", Border.BackgroundProperty, "XY.Brush.Accent.Soft"); VisualState(styles, typeof(XYSwitch), "xyui-switch", "xyui-switch-track", ":checked", Border.BorderBrushProperty, "XY.Brush.Border.Color.Selected"); VisualState(styles, typeof(XYSwitch), "xyui-switch", "xyui-switch-track", ":pointerover", Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); VisualState(styles, typeof(XYSwitch), "xyui-switch", "xyui-switch-track", ":focus", Border.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
        var thumb = Visual<XYSwitch, Ellipse>(styles, "xyui-switch", "xyui-switch-thumb"); Set(thumb, Shape.FillProperty, "XY.Brush.Surface.Raised"); styles.Add(thumb);
        VisualState(styles, typeof(XYSwitch), "xyui-switch", "xyui-switch-thumb", ":checked", Shape.FillProperty, "XY.Brush.Accent.Strong"); var moved = VisualStateStyle<XYSwitch, Ellipse>("xyui-switch", "xyui-switch-thumb", ":checked"); moved.Setters.Add(new Setter(global::Avalonia.Visual.RenderTransformProperty, new TranslateTransform(16, 0))); styles.Add(moved); Disabled(styles, typeof(XYSwitch), "xyui-switch");
    }

    static Style Visual<T, TVisual>(Styles styles, string host, string part) where T : global::Avalonia.StyledElement where TVisual : global::Avalonia.Visual => new(x => x.OfType<T>().Class(host).Descendant().OfType<TVisual>().Class(part));
    static Style VisualStateStyle<T, TVisual>(string host, string part, string state) where T : global::Avalonia.StyledElement where TVisual : global::Avalonia.Visual => new(x => x.OfType<T>().Class(host).Class(state).Descendant().OfType<TVisual>().Class(part));
    static void VisualState(Styles styles, Type type, string host, string part, string state, AvaloniaProperty property, string resource) { var s = new Style(x => x.OfType(type).Class(host).Class(state).Descendant().Class(part)); Set(s, property, resource); styles.Add(s); }
    static void MarkBase(Styles styles, Type type, string host, string part) { var s = new Style(x => x.OfType(type).Class(host).Descendant().Class(part)); s.Setters.Add(new Setter(global::Avalonia.Visual.OpacityProperty, 0d)); styles.Add(s); }
    static void MarkState(Styles styles, Type type, string host, string part, string state, AvaloniaProperty property, string resource) { var s = new Style(x => x.OfType(type).Class(host).Class(state).Descendant().Class(part)); Set(s, property, resource); s.Setters.Add(new Setter(global::Avalonia.Visual.OpacityProperty, 1d)); styles.Add(s); }
    static void Disabled(Styles styles, Type type, string host) { var s = new Style(x => x.OfType(type).Class(host).Class(":disabled")); s.Setters.Add(new Setter(global::Avalonia.Visual.OpacityProperty, 0.55)); styles.Add(s); }
}
