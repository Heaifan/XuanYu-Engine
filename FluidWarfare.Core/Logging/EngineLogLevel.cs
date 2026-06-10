namespace FluidWarfare.Core.Logging;

public enum EngineLogLevel
{
    Trace,
    Info,
    Warning,
    Error,
    Critical
}

public static class EngineLogLevelExtensions
{
    public static string ToChineseLabel(this EngineLogLevel level)
    {
        return level switch
        {
            EngineLogLevel.Trace => "【追踪】",
            EngineLogLevel.Info => "【信息】",
            EngineLogLevel.Warning => "【警告】",
            EngineLogLevel.Error => "【报错】",
            EngineLogLevel.Critical => "【严重】",
            _ => "【信息】"
        };
    }
}
