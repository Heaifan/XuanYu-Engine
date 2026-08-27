using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;
using HAlign = global::Avalonia.Layout.HorizontalAlignment;
using VAlign = global::Avalonia.Layout.VerticalAlignment;

namespace XYUI.Avalonia.Controls;

public enum XyuiVectorMarkPlacement { Inline, TopRight, BottomRight, Background }

public abstract class XyuiVectorTextSurface : XyuiTextSurface
{
    protected readonly VectorPath VectorMark;
    protected readonly Grid ContentGrid = new();
    protected virtual double CornerMarkStrokeThickness => 1.0;
    protected virtual double CornerMarkTextGap => 0;

    protected XyuiVectorTextSurface(string className, XyuiVectorIcon icon,
        XyuiVectorMarkPlacement placement, VectorPath? vectorMark = null) : base(className)
    {
        VectorMark = vectorMark ?? new VectorPath();
        VectorMark.Classes.Add($"{className}-mark");
        SetIcon(icon);
        AttachedToVisualTree += (_, _) => SetIcon(icon);
        VectorMark.Stretch = Stretch.Uniform;
        BuildLayout(placement);
    }

    void SetIcon(XyuiVectorIcon icon)
    {
        if (VectorMark.Data is null && XyuiVectorIcons.IsPlatformReady) VectorMark.Data = XyuiVectorIcons.Create(icon);
    }

    void BuildLayout(XyuiVectorMarkPlacement placement)
    {
        Child = null;
        if (placement == XyuiVectorMarkPlacement.Inline)
        {
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            VectorMark.Width = 14; VectorMark.Height = 14; VectorMark.VerticalAlignment = VAlign.Center;
            TextPresenter.Margin = new Thickness(6, 0, 0, 0);
            Grid.SetColumn(VectorMark, 0); Grid.SetColumn(TextPresenter, 1);
        }
        else if (placement is XyuiVectorMarkPlacement.TopRight or XyuiVectorMarkPlacement.BottomRight)
        {
            var overlay = new Canvas { IsHitTestVisible = false, HorizontalAlignment = HAlign.Stretch, VerticalAlignment = VAlign.Stretch };
            VectorMark.Width = 8; VectorMark.Height = 8; VectorMark.StrokeThickness = CornerMarkStrokeThickness; VectorMark.IsHitTestVisible = false;
            Canvas.SetRight(VectorMark, 6); TextPresenter.Margin = new Thickness(0, 0, 14 + CornerMarkTextGap, 0);
            if (placement == XyuiVectorMarkPlacement.TopRight) Canvas.SetTop(VectorMark, 5); else Canvas.SetBottom(VectorMark, 5);
            overlay.Children.Add(VectorMark); ContentGrid.Children.Add(TextPresenter); ContentGrid.Children.Add(overlay); Child = ContentGrid; return;
        }
        if (placement == XyuiVectorMarkPlacement.Background)
        {
            VectorMark.Stretch = Stretch.Fill;
            VectorMark.HorizontalAlignment = HAlign.Stretch;
            VectorMark.VerticalAlignment = VAlign.Stretch;
            VectorMark.Width = double.NaN; VectorMark.Height = double.NaN; TextPresenter.Margin = new Thickness(10, 2, 5, 2);
            ContentGrid.Children.Add(VectorMark); ContentGrid.Children.Add(TextPresenter);
        }
        else
        {
            ContentGrid.Children.Add(TextPresenter); ContentGrid.Children.Add(VectorMark);
        }
        Child = ContentGrid;
    }
}
