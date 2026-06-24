using XuanYu.Engine.Core.Results;

namespace FluidWarfare.Tests.Core.Results;

public sealed class EngineErrorTests
{
    [Fact]
    public void Create_WithValidCodeAndMessage_ShouldCreateError()
    {
        var error = EngineError.Create("Core.InvalidArgument", "参数无效。");

        Assert.Equal("Core.InvalidArgument", error.Code);
        Assert.Equal("参数无效。", error.Message);
        Assert.True(error.IsValid);
    }

    [Fact]
    public void Create_WithEmptyCode_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => EngineError.Create(string.Empty, "参数无效。"));
    }

    [Fact]
    public void Create_WithWhitespaceCode_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => EngineError.Create("   ", "参数无效。"));
    }

    [Fact]
    public void Create_WithEmptyMessage_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => EngineError.Create("Core.InvalidArgument", string.Empty));
    }

    [Fact]
    public void Create_WithWhitespaceMessage_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => EngineError.Create("Core.InvalidArgument", "   "));
    }

    [Fact]
    public void Default_ShouldBeInvalid()
    {
        Assert.False(default(EngineError).IsValid);
    }

    [Fact]
    public void SameValue_ShouldBeEqual()
    {
        Assert.Equal(CreateSample(), CreateSample());
    }

    [Fact]
    public void DifferentCode_ShouldNotBeEqual()
    {
        Assert.NotEqual(CreateSample(), EngineError.Create("Core.Other", "参数无效。"));
    }

    [Fact]
    public void DifferentMessage_ShouldNotBeEqual()
    {
        Assert.NotEqual(CreateSample(), EngineError.Create("Core.InvalidArgument", "字段缺失。"));
    }

    [Fact]
    public void ToString_ShouldBeStable()
    {
        Assert.Equal("Core.InvalidArgument：参数无效。", CreateSample().ToString());
    }

    [Fact]
    public void Message_ShouldNotContainLogLevelPrefix()
    {
        var message = CreateSample().Message;

        Assert.DoesNotContain("[追踪]", message);
        Assert.DoesNotContain("[报错]", message);
        Assert.DoesNotContain("[信息]", message);
        Assert.DoesNotContain("[警告]", message);
        Assert.DoesNotContain("[严重]", message);
    }

    private static EngineError CreateSample()
    {
        return EngineError.Create("Core.InvalidArgument", "参数无效。");
    }
}
