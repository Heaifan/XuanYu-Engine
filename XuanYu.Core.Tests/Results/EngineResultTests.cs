using XuanYu.Core.Results;

namespace XuanYu.Core.Tests.Results;

public sealed class EngineResultTests
{
    [Fact]
    public void Success_is_valid_and_has_no_error()
    {
        var result = EngineResult.Success();

        Assert.True(result.IsValid);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
        Assert.Equal("EngineResult(成功)", result.ToString());
    }

    [Fact]
    public void Fail_carries_error_and_rejects_invalid_error()
    {
        var error = EngineError.Create("EXAMPLE", "示例错误。");
        var result = EngineResult.Fail(error);

        Assert.True(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal("EngineResult(失败：EXAMPLE：示例错误。)", result.ToString());
        Assert.Throws<ArgumentException>(() => EngineResult.Fail(default));
    }

    [Fact]
    public void Default_result_is_invalid_not_a_failure()
    {
        var result = default(EngineResult);

        Assert.False(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
        Assert.Equal("EngineResult(无效)", result.ToString());
    }
}
