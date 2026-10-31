using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control DragFeedbackPreview() => DragCanvas("拖拽反馈 · Source + Ghost Preview");

    static Control DragFeedbackLiveExample()
    {
        var canvas = DragCanvas("拖拽视觉跟随 Pointer；Drop 位置由后续指示组件负责");
        var toggle = new XYButton { Content = "切换 Drag Feedback", Variant = XyuiButtonVariant.Secondary };
        var feedback = canvas.Children.OfType<XYDragFeedback>().Single();
        toggle.Click += (_, _) => feedback.IsDragging = !feedback.IsDragging;
        return new StackPanel { Spacing = 10, Children = { canvas, toggle } };
    }

    static Canvas DragCanvas(string caption)
    {
        var canvas = new Canvas { Width = 360, Height = 150, Background = Brushes.Transparent };
        var source = new Border { Width = 96, Height = 32, Background = Brushes.Transparent, BorderBrush = Brushes.SlateGray, BorderThickness = new Thickness(1) };
        Canvas.SetLeft(source, 32); Canvas.SetTop(source, 42); canvas.Children.Add(source);
        var feedback = new XYDragFeedback
        {
            IsDragging = true, SourceRect = new Rect(32, 42, 96, 32), PreviewRect = new Rect(210, 64, 96, 32)
        };
        canvas.Children.Add(feedback);
        canvas.Children.Add(new XYCaption { Text = "Source · 弱化但保留占位", [Canvas.LeftProperty] = 24, [Canvas.TopProperty] = 82 });
        canvas.Children.Add(new XYCaption { Text = "Ghost Preview", [Canvas.LeftProperty] = 208, [Canvas.TopProperty] = 106 });
        canvas.Children.Add(new TextBlock { Text = caption, Classes = { "xyui-text-caption" }, [Canvas.LeftProperty] = 12, [Canvas.TopProperty] = 8 });
        var grabOffset = new Point();
        source.PointerPressed += (_, e) =>
        {
            if (!e.GetCurrentPoint(source).Properties.IsLeftButtonPressed) return;
            grabOffset = e.GetPosition(source); var point = e.GetPosition(canvas);
            feedback.PreviewRect = new Rect(point.X - grabOffset.X, point.Y - grabOffset.Y, 96, 32);
            feedback.IsDragging = true; e.Pointer.Capture(source); e.Handled = true;
        };
        source.PointerMoved += (_, e) =>
        {
            if (e.Pointer.Captured != source) return;
            var point = e.GetPosition(canvas);
            feedback.PreviewRect = new Rect(point.X - grabOffset.X, point.Y - grabOffset.Y, 96, 32); e.Handled = true;
        };
        source.PointerReleased += (_, e) => { if (e.Pointer.Captured != source) return; e.Pointer.Capture(null); feedback.IsDragging = false; e.Handled = true; };
        source.PointerCaptureLost += (_, _) => feedback.IsDragging = false;
        return canvas;
    }
}
