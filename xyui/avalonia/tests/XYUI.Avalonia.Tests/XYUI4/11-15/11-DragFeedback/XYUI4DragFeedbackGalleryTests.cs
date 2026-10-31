using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI4DragFeedbackGalleryTests
{
    [Fact]
    public void Drag_feedback_is_registered_and_gallery_preview_is_live()
    {
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.11");

        Assert.IsType<Canvas>(preview);
        Assert.Single(preview.GetVisualDescendants().OfType<XYDragFeedback>());
        Assert.Contains(preview.GetVisualDescendants().OfType<Border>(), x => x.IsHitTestVisible);
        Assert.Contains(XYUI4DocumentationCatalog.Build(), x => x.Id == "XYUI-4-4.11");
    }
}
