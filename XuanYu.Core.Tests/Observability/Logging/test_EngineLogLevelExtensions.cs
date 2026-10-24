using XuanYu.Core.Logging;

namespace XuanYu.Core.Tests.Observability.Logging;

public sealed class EngineLogLevelExtensionsTests
{
    [Theory]
    [InlineData(EngineLogLevel.Trace, "[追踪]")]
    [InlineData(EngineLogLevel.Info, "[信息]")]
    [InlineData(EngineLogLevel.Warning, "[警告]")]
    [InlineData(EngineLogLevel.Error, "[报错]")]
    [InlineData(EngineLogLevel.Critical, "[严重]")]
    public void MapsChineseLabels(EngineLogLevel level, string expected)
    {
        Assert.Equal(expected, level.ToChineseLabel());
    }

    [Fact]
    public void RejectsUnknownLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ((EngineLogLevel)99).ToChineseLabel());
    }
}
