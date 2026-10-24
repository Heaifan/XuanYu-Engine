using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class ViewportStateTests
{
    [Fact]
    public void Accepts_valid_logical_physical_dpi_and_revision()
    {
        var viewport = new ViewportState(10.0, 20.0, 800.0, 600.0, 1600, 1200, 2.0, 7);

        Assert.Equal(800.0, viewport.LogicalWidth);
        Assert.Equal(1200, viewport.PhysicalHeight);
        Assert.Equal(7, viewport.Revision);
    }

    [Fact]
    public void Same_state_is_idempotent_and_revision_changes_identity()
    {
        var viewport = new ViewportState(0.0, 0.0, 800.0, 600.0, 800, 600, 1.0, 1);

        Assert.Equal(viewport, new ViewportState(0.0, 0.0, 800.0, 600.0, 800, 600, 1.0, 1));
        Assert.NotEqual(viewport, viewport.WithRevision(2));
    }

    [Fact]
    public void Rejects_invalid_dimensions_dpi_and_nan()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 0, 600, 800, 600, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 800, -1, 800, 600, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 800, 600, 0, 600, 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ViewportState(0, 0, 800, 600, 800, 600, double.NaN, 0));
    }
}
