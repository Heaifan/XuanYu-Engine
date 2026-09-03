using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class AdaptiveLayoutRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fixture;
    public AdaptiveLayoutRuntimeTests(XyuiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Responsive_uses_container_available_width_for_columns() => _fixture.Run(() =>
    {
        var layout = CreateLayout();
        var window = Show(layout, 900);
        Assert.Equal(3, layout.CurrentColumnCount);
        window.Width = 500; Dispatcher.UIThread.RunJobs();
        Assert.Equal(1, layout.CurrentColumnCount);
        window.Close();
    });

    [Fact]
    public void Reflow_preserves_children_and_does_not_change_size_or_density() => _fixture.Run(() =>
    {
        var layout = CreateLayout();
        XY.SetSize(layout, XYSize.Comfortable); XY.SetDensity(layout, XYDensity.Compact);
        var window = Show(layout, 900);
        var sizes = layout.Children.Select(child => child.Bounds.Size).ToArray();
        window.Width = 560; Dispatcher.UIThread.RunJobs();
        Assert.Equal(3, layout.Children.Count);
        Assert.Equal(XYSize.Comfortable, XY.GetSize(layout));
        Assert.Equal(XYDensity.Compact, XY.GetDensity(layout));
        Assert.Equal(sizes[0].Height, layout.Children[0].Bounds.Height);
        window.Close();
    });

    [Fact]
    public void Column_count_clamps_to_one_and_max_columns_without_window_width() => _fixture.Run(() =>
    {
        var layout = new AdaptiveLayout { MinItemWidth = 280, MaxColumns = 3 };
        Assert.Equal(1, layout.CalculateColumnCount(100));
        Assert.Equal(2, layout.CalculateColumnCount(579));
        Assert.Equal(3, layout.CalculateColumnCount(900));
    });

    [Fact]
    public void Gap_is_consumed_from_existing_xy_gap_facade() => _fixture.Run(() =>
    {
        var layout = new AdaptiveLayout { MinItemWidth = 280, MaxColumns = 3 };
        XY.SetGap(layout, "XY.Space.2");
        Assert.Equal(2, layout.CalculateColumnCount(568));
        Assert.Equal(1, layout.CalculateColumnCount(567));
    });

    static AdaptiveLayout CreateLayout() => new()
    {
        MinItemWidth = 280, MaxColumns = 3,
        Children = { new Border { Width = 120, Height = 40 }, new Border { Width = 120, Height = 60 }, new Border { Width = 120, Height = 50 } },
    };

    static Window Show(AdaptiveLayout layout, double width)
    {
        XyuiBatchTestHost.Prepare();
        var window = new Window { Width = width, Height = 240, Content = layout };
        window.Show(); layout.ApplyStyling(); Dispatcher.UIThread.RunJobs(); return window;
    }
}
