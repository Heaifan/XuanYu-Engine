using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

// MAP-A-R2-D3-F2：日志自动跟随纯策略——底部附近跟随、远离不强制拉回、滚到底恢复。
public sealed class LogAutoScrollPolicyTests
{
    [Fact]
    public void At_bottom_follows()
    {
        Assert.True(LogAutoScrollPolicy.ShouldFollow(offset: 480, maxOffset: 500));
        Assert.True(LogAutoScrollPolicy.ShouldFollow(offset: 500, maxOffset: 500));
    }
    [Fact]
    public void Near_bottom_within_threshold_follows()
    {
        Assert.True(LogAutoScrollPolicy.ShouldFollow(
            offset: 500 - LogAutoScrollPolicy.FollowThresholdDips, maxOffset: 500));
    }
    [Fact]
    public void Far_from_bottom_does_not_follow()
    {
        Assert.False(LogAutoScrollPolicy.ShouldFollow(offset: 100, maxOffset: 500));
        Assert.False(LogAutoScrollPolicy.ShouldFollow(offset: 400, maxOffset: 500));
    }
    [Fact]
    public void No_scrollable_range_always_follows()
    {
        Assert.True(LogAutoScrollPolicy.ShouldFollow(offset: 0, maxOffset: 0));
        Assert.True(LogAutoScrollPolicy.ShouldFollow(offset: 0, maxOffset: -10));
    }
}
