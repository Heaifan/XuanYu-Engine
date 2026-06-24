namespace XuanYu.Engine.Core.Logging;

public readonly record struct EngineLogEntry
{
    private static readonly string[] LogLevelPrefixes =
    [
        EngineLogLevelExtensions.TraceChineseLabel,
        EngineLogLevelExtensions.InfoChineseLabel,
        EngineLogLevelExtensions.WarningChineseLabel,
        EngineLogLevelExtensions.ErrorChineseLabel,
        EngineLogLevelExtensions.CriticalChineseLabel
    ];

    private EngineLogEntry(
        double simulationSeconds,
        EngineLogLevel level,
        string category,
        string message)
    {
        SimulationSeconds = simulationSeconds;
        Level = level;
        Category = category;
        Message = message;
    }

    public double SimulationSeconds { get; }

    public EngineLogLevel Level { get; }

    public string Category { get; }

    public string Message { get; }

    public static EngineLogEntry Create(
        double simulationSeconds,
        EngineLogLevel level,
        string category,
        string message)
    {
        if (!double.IsFinite(simulationSeconds) || simulationSeconds < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(simulationSeconds), simulationSeconds, "模拟时间必须是有限数，并且不能为负数。");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("日志分类不能为空。", nameof(category));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("日志内容不能为空。", nameof(message));
        }

        if (ContainsLogLevelPrefix(message))
        {
            throw new ArgumentException("日志内容不应包含日志等级前缀。", nameof(message));
        }

        return new EngineLogEntry(simulationSeconds, level, category, message);
    }

    public string ToDisplayString()
    {
        return $"{Level.ToChineseLabel()}{Message}";
    }

    public override string ToString()
    {
        return ToDisplayString();
    }

    private static bool ContainsLogLevelPrefix(string message)
    {
        foreach (var prefix in LogLevelPrefixes)
        {
            if (message.Contains(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
