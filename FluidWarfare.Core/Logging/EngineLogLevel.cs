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
    internal const string TraceChineseLabel = "[追踪]";
    internal const string InfoChineseLabel = "[信息]";
    internal const string WarningChineseLabel = "[警告]";
    internal const string ErrorChineseLabel = "[报错]";
    internal const string CriticalChineseLabel = "[严重]";

    public static string ToChineseLabel(this EngineLogLevel level)
    {
        return level switch
        {
            EngineLogLevel.Trace => TraceChineseLabel,
            EngineLogLevel.Info => InfoChineseLabel,
            EngineLogLevel.Warning => WarningChineseLabel,
            EngineLogLevel.Error => ErrorChineseLabel,
            EngineLogLevel.Critical => CriticalChineseLabel,
            _ => InfoChineseLabel
        };
    }
}
