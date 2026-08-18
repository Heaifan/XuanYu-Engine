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
    protected readonly VectorPath VectorMark = new();
    protected readonly Grid ContentGrid = new();

    protected XyuiVectorTextSurface(string className, XyuiVectorIcon icon, XyuiVectorMarkPlacement placement) : base(className)
    {
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
        else
        {
            VectorMark.HorizontalAlignment = HAlign.Right;
            VectorMark.VerticalAlignment = placement == XyuiVectorMarkPlacement.TopRight ? VAlign.Top : VAlign.Bottom;
            VectorMark.Width = 14; VectorMark.Height = 14;
            VectorMark.Margin = placement == XyuiVectorMarkPlacement.TopRight ? new Thickness(0, 2, 4, 0) : new Thickness(0, 0, 4, 2);
            TextPresenter.Margin = placement == XyuiVectorMarkPlacement.TopRight ? new Thickness(0, 0, 18, 0) : new Thickness(0, 0, 18, 2);
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
