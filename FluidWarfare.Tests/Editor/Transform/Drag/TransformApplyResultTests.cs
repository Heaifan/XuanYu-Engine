using FluidWarfare.Editor.Windows.Viewport.Transform.Application;

namespace FluidWarfare.Tests.Editor.Transform.Drag;

public sealed class TransformApplyResultTests
{
    [Fact]
    public void SuccessResult_HasStatusSuccess()
    {
        var r = TransformApplyResult.SuccessResult;
        Assert.Equal(TransformApplyStatus.Success, r.Status);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public void NoChangeResult_HasStatusNoChange()
    {
        var r = TransformApplyResult.NoChangeResult;
        Assert.Equal(TransformApplyStatus.NoChange, r.Status);
        Assert.False(r.IsSuccess);
    }

    [Fact]
    public void FailureResult_HasFailureReason()
    {
        var r = TransformApplyResult.Failure(TransformFailureReason.RenderSceneSyncFailed);
        Assert.Equal(TransformApplyStatus.Failure, r.Status);
        Assert.Equal(TransformFailureReason.RenderSceneSyncFailed, r.FailureReason);
        Assert.False(r.IsSuccess);
    }
}
