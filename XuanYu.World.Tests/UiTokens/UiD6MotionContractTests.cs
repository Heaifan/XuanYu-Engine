using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD6MotionContractTests
{
    [Fact]
    public void Reduce_motion_removes_nonessential_transitions()
    {
        Assert.False(UiMotionContract.AllowsNonEssentialTransitions(
            UiMotionPreference.Reduce));
        Assert.Equal(0, UiMotionContract.EffectiveHoverMs(
            UiMotionPreference.Reduce));
        Assert.Equal(0, UiMotionContract.EffectiveDialogMs(
            UiMotionPreference.Reduce));
    }

    [Fact]
    public void Default_motion_keeps_short_editor_feedback()
    {
        Assert.True(UiMotionContract.AllowsNonEssentialTransitions(
            UiMotionPreference.Default));
        Assert.Equal(UiMotionContract.FastMs, UiMotionContract.EffectiveHoverMs(
            UiMotionPreference.Default));
        Assert.Equal(UiMotionContract.StandardMs, UiMotionContract.EffectiveDialogMs(
            UiMotionPreference.Default));
        Assert.True(UiMotionContract.SlowMs <= 180);
    }
}
