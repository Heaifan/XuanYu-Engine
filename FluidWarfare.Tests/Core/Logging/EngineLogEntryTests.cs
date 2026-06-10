using FluidWarfare.Core.Logging;

namespace FluidWarfare.Tests.Core.Logging;

public sealed class EngineLogEntryTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateEntry()
    {
        var entry = CreateSample();

        Assert.Equal(1.5, entry.SimulationSeconds);
        Assert.Equal(EngineLogLevel.Info, entry.Level);
        Assert.Equal("Test", entry.Category);
        Assert.Equal("测试完成。", entry.Message);
    }

    [Fact]
    public void Create_WithNegativeSimulationSeconds_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            EngineLogEntry.Create(-0.1, EngineLogLevel.Info, "Test", "测试完成。"));

        Assert.Contains("模拟时间必须是有限数，并且不能为负数。", exception.Message);
    }

    [Fact]
    public void Create_WithNaNSeconds_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            EngineLogEntry.Create(double.NaN, EngineLogLevel.Info, "Test", "测试完成。"));

        Assert.Contains("模拟时间必须是有限数，并且不能为负数。", exception.Message);
    }

    [Fact]
    public void Create_WithInfinitySeconds_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            EngineLogEntry.Create(double.PositiveInfinity, EngineLogLevel.Info, "Test", "测试完成。"));

        Assert.Contains("模拟时间必须是有限数，并且不能为负数。", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyCategory_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Info, string.Empty, "测试完成。"));

        Assert.Contains("日志分类不能为空。", exception.Message);
    }

    [Fact]
    public void Create_WithWhitespaceCategory_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Info, "   ", "测试完成。"));

        Assert.Contains("日志分类不能为空。", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyMessage_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Info, "Test", string.Empty));

        Assert.Contains("日志内容不能为空。", exception.Message);
    }

    [Fact]
    public void Create_WithWhitespaceMessage_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Info, "Test", "   "));

        Assert.Contains("日志内容不能为空。", exception.Message);
    }

    [Fact]
    public void Create_WithMessageContainingInfoPrefix_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Info, "Test", "【信息】测试完成。"));

        Assert.Contains("日志内容不应包含日志等级前缀。", exception.Message);
    }

    [Fact]
    public void Create_WithMessageContainingErrorPrefix_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Error, "Core", "【报错】参数无效。"));

        Assert.Contains("日志内容不应包含日志等级前缀。", exception.Message);
    }

    [Theory]
    [InlineData("【追踪】")]
    [InlineData("【信息】")]
    [InlineData("【警告】")]
    [InlineData("【报错】")]
    [InlineData("【严重】")]
    public void Create_WithMessageContainingAnyCurrentPrefix_ShouldThrow(string prefix)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(1.5, EngineLogLevel.Info, "Test", $"{prefix}测试完成。"));

        Assert.Contains("日志内容不应包含日志等级前缀。", exception.Message);
    }

    [Fact]
    public void ToDisplayString_ForInfo_ShouldUseChinesePrefix()
    {
        Assert.Equal("【信息】测试完成。", CreateSample().ToDisplayString());
    }

    [Fact]
    public void ToDisplayString_ForError_ShouldUseChinesePrefix()
    {
        var entry = EngineLogEntry.Create(2.0, EngineLogLevel.Error, "Core", "参数无效。");

        Assert.Equal("【报错】参数无效。", entry.ToDisplayString());
    }

    [Fact]
    public void ToString_ShouldMatchDisplayString()
    {
        var entry = CreateSample();

        Assert.Equal(entry.ToDisplayString(), entry.ToString());
    }

    [Fact]
    public void SameValue_ShouldBeEqual()
    {
        Assert.Equal(CreateSample(), CreateSample());
    }

    [Fact]
    public void DifferentMessage_ShouldNotBeEqual()
    {
        Assert.NotEqual(CreateSample(), EngineLogEntry.Create(1.5, EngineLogLevel.Info, "Test", "字段缺失。"));
    }

    private static EngineLogEntry CreateSample()
    {
        return EngineLogEntry.Create(1.5, EngineLogLevel.Info, "Test", "测试完成。");
    }
}
