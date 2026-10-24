using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class ViewportTests
{
    [Fact]
    public void AcceptsDimensionsAndDpi()
    {
        var viewport = new ViewportState(10.0, 20.0, 800.0, 600.0, 1600, 1200, 2.0, 7);

        Assert.Equal(800.0, viewport.LogicalWidth);
        Assert.Equal(1200, viewport.PhysicalHeight);
        Assert.Equal(7, viewport.Revision);
    }

    [Fact]
    public void RevisionChangesIdentity()
    {
        var viewport = new ViewportState(0.0, 0.0, 800.0, 600.0, 800, 600, 1.0, 1);

        Assert.Equal(viewport, new ViewportState(0.0, 0.0, 800.0, 600.0, 800, 600, 1.0, 1));
        Assert.NotEqual(viewport, viewport.WithRevision(2));
    }

    [Fact]
    public void RejectsInvalidDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 0, 600, 800, 600, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 800, -1, 800, 600, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 800, 600, 0, 600, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 800, 600, 800, 600, double.NaN, 0));
    }
}
