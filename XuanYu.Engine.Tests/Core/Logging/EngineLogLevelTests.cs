using XuanYu.Engine.Core.Logging;

namespace FluidWarfare.Tests.Core.Logging;

public sealed class EngineLogLevelTests
{
    [Fact]
    public void Trace_ShouldReturnChineseLabel()
    {
        Assert.Equal("[追踪]", EngineLogLevel.Trace.ToChineseLabel());
    }

    [Fact]
    public void Info_ShouldReturnChineseLabel()
    {
        Assert.Equal("[信息]", EngineLogLevel.Info.ToChineseLabel());
    }

    [Fact]
    public void Warning_ShouldReturnChineseLabel()
    {
        Assert.Equal("[警告]", EngineLogLevel.Warning.ToChineseLabel());
    }

    [Fact]
    public void Error_ShouldReturnChineseLabel()
    {
        Assert.Equal("[报错]", EngineLogLevel.Error.ToChineseLabel());
    }

    [Fact]
    public void Critical_ShouldReturnChineseLabel()
    {
        Assert.Equal("[严重]", EngineLogLevel.Critical.ToChineseLabel());
    }
}
