using XuanYu.Core.Logging;

namespace XuanYu.Core.Tests.Foundation;

public sealed class EngineLogContractTests
{
    [Fact]
    public void Level_prefix_at_start_is_rejected()
    {
        Assert.Throws<ArgumentException>(() =>
            EngineLogEntry.Create(0, EngineLogLevel.Info, "Core", "[警告]重复等级"));
    }

    [Fact]
    public void Level_label_quoted_inside_message_is_allowed()
    {
        var log = EngineLogEntry.Create(0, EngineLogLevel.Info,
            "Core", "用户引用了[警告]标签");
        Assert.Equal("[信息]用户引用了[警告]标签", log.ToDisplayString());
    }

    [Fact]
    public void Undefined_level_is_rejected_when_creating_log()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EngineLogEntry.Create(0, (EngineLogLevel)456, "Core", "正文"));
    }

    [Fact]
    public void Undefined_level_is_not_silently_formatted_as_info()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ((EngineLogLevel)456).ToChineseLabel());
    }

    [Fact]
    public void All_known_log_levels_keep_existing_labels()
    {
        Assert.Equal("[追踪]", EngineLogLevel.Trace.ToChineseLabel());
        Assert.Equal("[信息]", EngineLogLevel.Info.ToChineseLabel());
        Assert.Equal("[警告]", EngineLogLevel.Warning.ToChineseLabel());
        Assert.Equal("[报错]", EngineLogLevel.Error.ToChineseLabel());
        Assert.Equal("[严重]", EngineLogLevel.Critical.ToChineseLabel());
    }
}
