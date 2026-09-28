using Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI4DragFeedbackTests
{
    [Fact]
    public void Drag_feedback_is_a_non_interactive_overlay()
    {
        var feedback = new XYDragFeedback();

        Assert.Equal("XYUI-4-4.11", feedback.CanonicalId);
        Assert.False(feedback.IsHitTestVisible);
        Assert.Equal(0, feedback.DesiredSize.Width);
        Assert.Equal(0, feedback.DesiredSize.Height);
    }

    [Fact]
    public void Drag_feedback_keeps_preview_and_source_geometry_explicit()
    {
        var feedback = new XYDragFeedback
        {
            SourceRect = new Rect(10, 12, 80, 24),
            PreviewRect = new Rect(140, 160, 80, 24),
            IsDragging = true,
            PreviewText = "对象"
        };

        Assert.True(feedback.IsDragging);
        Assert.Equal(new Rect(10, 12, 80, 24), feedback.SourceRect);
        Assert.Equal(new Rect(140, 160, 80, 24), feedback.PreviewRect);
        Assert.Equal("对象", feedback.PreviewText);
    }
}
