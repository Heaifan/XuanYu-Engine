using XuanYu.Core.Results;

namespace XuanYu.Core.Tests.Results;

public sealed class EngineErrorTests
{
    [Fact]
    public void Create_keeps_code_and_message()
    {
        var error = EngineError.Create("EXAMPLE", "示例错误。");

        Assert.True(error.IsValid);
        Assert.Equal("EXAMPLE", error.Code);
        Assert.Equal("示例错误。", error.Message);
        Assert.Equal("EXAMPLE：示例错误。", error.ToString());
    }

    [Fact]
    public void Create_rejects_blank_code_or_message()
    {
        Assert.Throws<ArgumentException>(() => EngineError.Create("", "错误。"));
        Assert.Throws<ArgumentException>(() => EngineError.Create("EXAMPLE", " "));
    }

    [Fact]
    public void Default_error_is_invalid_and_displays_invalid()
    {
        var error = default(EngineError);

        Assert.False(error.IsValid);
        Assert.Equal("EngineError(无效)", error.ToString());
    }
}
