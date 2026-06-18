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
}
