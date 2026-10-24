using XuanYu.Core.Results;

namespace XuanYu.Core.Tests.Results;

public sealed class EngineResultTests
{
    [Fact]
    public void SuccessHasNoError()
    {
        var result = EngineResult.Success();

        Assert.True(result.IsValid);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
        Assert.Equal("EngineResult(成功)", result.ToString());
    }

    [Fact]
    public void FailKeepsError()
    {
        var error = EngineError.Create("EXAMPLE", "示例错误。");
        var result = EngineResult.Fail(error);

        Assert.True(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal("EngineResult(失败：EXAMPLE：示例错误。)", result.ToString());
    }

    [Fact]
    public void RejectsInvalidError()
    {
        Assert.Throws<ArgumentException>(() => EngineResult.Fail(default));
    }

    [Fact]
    public void DefaultIsInvalid()
    {
        var result = default(EngineResult);

        Assert.False(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
        Assert.Equal("EngineResult(无效)", result.ToString());
    }
}
