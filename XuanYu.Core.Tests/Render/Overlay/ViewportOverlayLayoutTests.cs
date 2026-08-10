using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Overlay;

public sealed class ViewportOverlayLayoutTests
{
    [Theory]
    [InlineData(1024, 640)]
    [InlineData(1360, 820)]
    [InlineData(1920, 1080)]
    public void Bottom_left_scale_rect_stays_inside_viewport(double width, double height)
    {
        var rect = ViewportOverlayLayoutResolver.Resolve(new(
            width, height, 168, 38, 16, 16, ViewportOverlayAnchor.BottomLeft));

        Assert.Equal(16, rect.X);
        Assert.Equal(height - 54, rect.Y);
        Assert.True(rect.Right <= width);
        Assert.True(rect.Bottom <= height);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    public void Top_right_gizmo_anchor_is_dpi_independent(double dpi)
    {
        var request = new ViewportOverlayLayoutRequest(
            1360, 820, 96, 96, 14, 14, ViewportOverlayAnchor.TopRight);
        var rect = ViewportOverlayLayoutResolver.Resolve(request);

        Assert.Equal(new ViewportOverlayRect(1250, 14, 96, 96), rect);
        Assert.Equal(1250 * dpi, rect.X * dpi);
        Assert.Equal(14 * dpi, rect.Y * dpi);
    }

    [Fact]
    public void Bottom_right_anchor_uses_the_same_margin_semantics()
    {
        var rect = ViewportOverlayLayoutResolver.Resolve(new(
            1024, 640, 168, 38, 16, 16, ViewportOverlayAnchor.BottomRight));
        Assert.Equal(new ViewportOverlayRect(840, 586, 168, 38), rect);
    }

    [Fact]
    public void Oversized_overlay_is_clamped_to_viewport()
    {
        var rect = ViewportOverlayLayoutResolver.Resolve(new(
            80, 30, 160, 40, 16, 16, ViewportOverlayAnchor.BottomRight));
        Assert.Equal(new ViewportOverlayRect(0, 0, 80, 30), rect);
    }

    [Fact]
    public void Scale_indicator_width_cap_keeps_normal_viewport_margin()
    {
        var rect = ViewportOverlayLayoutResolver.Resolve(new(
            800, 600, 160, 32, 16, 16, ViewportOverlayAnchor.BottomLeft));

        Assert.Equal(160, rect.Width);
        Assert.Equal(16, rect.X);
        Assert.True(rect.Right < 800);
    }
}
