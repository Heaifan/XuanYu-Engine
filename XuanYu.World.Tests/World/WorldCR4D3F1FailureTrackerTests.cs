using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR4D3F1FailureTrackerTests
{
    [Fact]
    public void Same_key_and_revision_skips_after_first_failure()
    {
        var tracker = new VulkanStaticModelFailureTracker();
        var key = new RenderStaticModelKey("a");
        tracker.Record(key, 1);

        Assert.True(tracker.ShouldSkip(key, 1));
        Assert.Equal(1, tracker.Count);
    }

    [Fact]
    public void Revision_change_allows_retry()
    {
        var tracker = new VulkanStaticModelFailureTracker();
        var key = new RenderStaticModelKey("a");
        tracker.Record(key, 1);

        Assert.True(tracker.ShouldSkip(key, 1));
        Assert.False(tracker.ShouldSkip(key, 2));
    }

    [Fact]
    public void Clear_allows_retry_for_same_revision()
    {
        var tracker = new VulkanStaticModelFailureTracker();
        var key = new RenderStaticModelKey("a");
        tracker.Record(key, 1);
        tracker.Clear(key);

        Assert.False(tracker.ShouldSkip(key, 1));
        Assert.Equal(0, tracker.Count);
    }

    [Fact]
    public void Clear_not_in_removes_only_unreferenced_failures()
    {
        var tracker = new VulkanStaticModelFailureTracker();
        var keep = new RenderStaticModelKey("keep");
        var drop = new RenderStaticModelKey("drop");
        tracker.Record(keep, 1);
        tracker.Record(drop, 1);

        tracker.ClearNotIn([keep]);

        Assert.True(tracker.ShouldSkip(keep, 1));
        Assert.False(tracker.ShouldSkip(drop, 1));
        Assert.Equal(1, tracker.Count);
    }

    [Fact]
    public void Failures_are_tracked_per_key_independently()
    {
        var tracker = new VulkanStaticModelFailureTracker();
        var a = new RenderStaticModelKey("a");
        var b = new RenderStaticModelKey("b");
        tracker.Record(a, 1);

        Assert.True(tracker.ShouldSkip(a, 1));
        Assert.False(tracker.ShouldSkip(b, 1));
        Assert.Equal(1, tracker.Count);
    }
}
