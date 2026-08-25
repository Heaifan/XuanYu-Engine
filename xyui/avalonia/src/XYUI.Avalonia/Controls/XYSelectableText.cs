using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;
using HAlign = global::Avalonia.Layout.HorizontalAlignment;
using VAlign = global::Avalonia.Layout.VerticalAlignment;

namespace XYUI.Avalonia.Controls;

public enum XyuiSelectableTextVariant { Default, Technical }

public sealed class XYSelectableText : Border
{
    public const double CopyMarkSize = 8;
    public const double CopyMarkVisualSize = 8;
    public const double CopyMarkGap = XyuiSpatialTokens.Space2;
    readonly SelectableTextBlock _text = new();
    readonly VectorPath _copyMark = new();
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<XYSelectableText, string>(nameof(Text), "");
    public static readonly StyledProperty<XyuiSelectableTextVariant> VariantProperty =
        AvaloniaProperty.Register<XYSelectableText, XyuiSelectableTextVariant>(nameof(Variant), XyuiSelectableTextVariant.Default);

    public XYSelectableText()
    {
        Classes.Add("xyui-1-component"); Classes.Add("xyui-selectable-text"); ApplyVariant(Variant);
        _text.Classes.Add("xyui-selectable-text-content"); _copyMark.Classes.Add("xyui-selectable-copy-mark");
        if (XyuiVectorIcons.IsPlatformReady) _copyMark.Data = XyuiVectorIcons.Create(XyuiVectorIcon.Copy);
        AttachedToVisualTree += (_, _) => { if (_copyMark.Data is null) _copyMark.Data = XyuiVectorIcons.Create(XyuiVectorIcon.Copy); };
        _copyMark.Width = CopyMarkVisualSize; _copyMark.Height = CopyMarkVisualSize;
        _copyMark.Stretch = Stretch.Uniform;
        _copyMark.HorizontalAlignment = HAlign.Left; _copyMark.VerticalAlignment = VAlign.Center;
        _copyMark.IsHitTestVisible = false; _copyMark.IsVisible = false;
        var grid = new Grid { HorizontalAlignment = HAlign.Left };
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(CopyMarkGap, GridUnitType.Pixel));
        grid.ColumnDefinitions.Add(new ColumnDefinition(CopyMarkSize, GridUnitType.Pixel));
        Grid.SetColumn(_text, 0); Grid.SetColumn(_copyMark, 2);
        grid.Children.Add(_text); grid.Children.Add(_copyMark); Child = grid;
        PointerEntered += (_, _) => _copyMark.IsVisible = true; PointerExited += (_, _) => _copyMark.IsVisible = false;
    }

    public string CanonicalId => "XYUI-1-21";
    public string Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public XyuiSelectableTextVariant Variant { get => GetValue(VariantProperty); set { SetValue(VariantProperty, value); ApplyVariant(value); } }
    public XyuiVectorIcon CopyIcon => XyuiVectorIcon.Copy;
    public int SelectionStart { get => _text.SelectionStart; set => _text.SelectionStart = value; }
    public int SelectionEnd { get => _text.SelectionEnd; set => _text.SelectionEnd = value; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty) _text.Text = change.GetNewValue<string>();
        if (change.Property == VariantProperty) ApplyVariant(change.GetNewValue<XyuiSelectableTextVariant>());
    }

    void ApplyVariant(XyuiSelectableTextVariant value) => _text.Classes.Set("xyui-selectable-text-technical", value == XyuiSelectableTextVariant.Technical);
}
