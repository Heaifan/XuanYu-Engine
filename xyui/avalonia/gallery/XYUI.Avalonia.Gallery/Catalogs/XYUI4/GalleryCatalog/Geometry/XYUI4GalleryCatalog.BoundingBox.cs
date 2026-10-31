using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control BoundingBoxPreview() => new StackPanel
    {
        Spacing = 8,
        Children = { Box("Resize + Rotate + Pivot"), new TextBlock { Text = "BoundingBox 只在 Transform 工具上下文出现，不替代 SelectionOutline。" } }
    };

    static Control BoundingBoxLiveExample()
    {
        var panel = new StackPanel { Spacing = 8 };
        foreach (var child in LiveBox()) panel.Children.Add(child);
        return panel;
    }

    static Control[] LiveBox()
    {
        var box = Box("Transform handles");
        var status = new TextBlock { Text = "拖动角点/边中缩放；顶端旋转；中心点移动 Pivot。" };
        box.PropertyChanged += (_, e) => { if (e.Property == XYBoundingBox.BoundsRectProperty || e.Property == XYBoundingBox.AngleProperty || e.Property == XYBoundingBox.PivotProperty) status.Text = $"Bounds {box.BoundsRect.Width:0} × {box.BoundsRect.Height:0} · Angle {box.Angle:0}° · Pivot {box.Pivot.X:0},{box.Pivot.Y:0}"; };
        return [box, status];
    }

    static XYBoundingBox Box(string label) => new()
    {
        Width = 260, Height = 120, BoundsRect = new Rect(44, 34, 172, 54), Pivot = new Point(130, 61)
    };
}
