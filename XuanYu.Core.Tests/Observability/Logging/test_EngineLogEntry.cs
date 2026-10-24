using XuanYu.Core.Logging;

namespace XuanYu.Core.Tests.Observability.Logging;

public sealed class EngineLogEntryTests
{
    [Fact]
    public void CreatesEntry()
    {
        var entry = EngineLogEntry.Create(1.5, EngineLogLevel.Warning, "Core", "Ready");

        Assert.Equal(1.5, entry.SimulationSeconds);
        Assert.Equal(EngineLogLevel.Warning, entry.Level);
        Assert.Equal("Core", entry.Category);
        Assert.Equal("Ready", entry.Message);
        Assert.Equal("[警告]Ready", entry.ToDisplayString());
        Assert.Equal(entry.ToDisplayString(), entry.ToString());
    }

    [Fact]
    public void RejectsInvalidTime()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EngineLogEntry.Create(-1, EngineLogLevel.Info, "Core", "Ready"));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EngineLogEntry.Create(double.NaN, EngineLogLevel.Info, "Core", "Ready"));
    }

    [Fact]
    public void RejectsBlankFields()
    {
        Assert.Throws<ArgumentException>(
            () => EngineLogEntry.Create(0, EngineLogLevel.Info, " ", "Ready"));
        Assert.Throws<ArgumentException>(
            () => EngineLogEntry.Create(0, EngineLogLevel.Info, "Core", " "));
    }

    [Fact]
    public void RejectsLevelPrefix()
    {
        Assert.Throws<ArgumentException>(
            () => EngineLogEntry.Create(0, EngineLogLevel.Info, "Core", "[警告]Ready"));
        Assert.Throws<ArgumentException>(
            () => EngineLogEntry.Create(0, EngineLogLevel.Info, "Core", " [信息]Ready"));
    }

    [Fact]
    public void AllowsEmbeddedLabel()
    {
        var entry = EngineLogEntry.Create(0, EngineLogLevel.Info, "Core", "解析 [警告] 标签");

        Assert.Equal("[信息]解析 [警告] 标签", entry.ToDisplayString());
    }

    [Fact]
    public void RejectsInvalidLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EngineLogEntry.Create(0, (EngineLogLevel)99, "Core", "Ready"));
    }
}
