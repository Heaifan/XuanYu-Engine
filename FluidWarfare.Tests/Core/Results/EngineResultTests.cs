using FluidWarfare.Core.Results;

namespace FluidWarfare.Tests.Core.Results;

public sealed class EngineResultTests
{
    [Fact]
    public void Success_ShouldBeSuccessful()
    {
        Assert.True(EngineResult.Success().IsSuccess);
    }

    [Fact]
    public void Success_ShouldNotHaveError()
    {
        Assert.Null(EngineResult.Success().Error);
    }

    [Fact]
    public void Fail_WithValidError_ShouldBeFailure()
    {
        Assert.True(EngineResult.Fail(CreateSampleError()).IsFailure);
    }

    [Fact]
    public void Fail_WithValidError_ShouldExposeError()
    {
        Assert.Equal(CreateSampleError(), EngineResult.Fail(CreateSampleError()).Error);
    }

    [Fact]
    public void Fail_WithDefaultError_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => EngineResult.Fail(default));
    }

    [Fact]
    public void Default_ShouldBeInvalid()
    {
        Assert.False(default(EngineResult).IsValid);
    }

    [Fact]
    public void Default_ShouldNotBeSuccessful()
    {
        Assert.False(default(EngineResult).IsSuccess);
    }

    [Fact]
    public void Default_ShouldNotBeFailure()
    {
        Assert.False(default(EngineResult).IsFailure);
    }

    [Fact]
    public void IsFailure_ShouldBeOppositeOfIsSuccess()
    {
        var success = EngineResult.Success();
        var failure = EngineResult.Fail(CreateSampleError());

        Assert.NotEqual(success.IsSuccess, success.IsFailure);
        Assert.NotEqual(failure.IsSuccess, failure.IsFailure);
    }

    [Fact]
    public void Success_ToString_ShouldBeStable()
    {
        Assert.Equal("EngineResult(成功)", EngineResult.Success().ToString());
    }

    [Fact]
    public void Failure_ToString_ShouldBeStable()
    {
        Assert.Equal("EngineResult(失败：Core.InvalidArgument：参数无效。)", EngineResult.Fail(CreateSampleError()).ToString());
    }

    [Fact]
    public void Failure_ToString_ShouldNotContainLogLevelPrefix()
    {
        var text = EngineResult.Fail(CreateSampleError()).ToString();

        Assert.DoesNotContain("[追踪]", text);
        Assert.DoesNotContain("[报错]", text);
        Assert.DoesNotContain("[信息]", text);
        Assert.DoesNotContain("[警告]", text);
        Assert.DoesNotContain("[严重]", text);
    }

    [Fact]
    public void SameSuccessResults_ShouldBeEqual()
    {
        Assert.Equal(EngineResult.Success(), EngineResult.Success());
    }

    [Fact]
    public void SameFailureResults_ShouldBeEqual()
    {
        Assert.Equal(EngineResult.Fail(CreateSampleError()), EngineResult.Fail(CreateSampleError()));
    }

    [Fact]
    public void DifferentFailureResults_ShouldNotBeEqual()
    {
        Assert.NotEqual(EngineResult.Fail(CreateSampleError()), EngineResult.Fail(EngineError.Create("Core.Other", "字段缺失。")));
    }

    private static EngineError CreateSampleError()
    {
        return EngineError.Create("Core.InvalidArgument", "参数无效。");
    }
}
