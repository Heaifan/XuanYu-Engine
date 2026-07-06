namespace XuanYu.Editor.UI;

public static class EditorLogFilterQuery
{
    public static bool Allows(this EditorLogFilter filter, LogEntry entry) => filter switch
    {
        EditorLogFilter.Info => entry.Level == EditorLogLevel.Info,
        EditorLogFilter.Warning => entry.Level == EditorLogLevel.Warning,
        EditorLogFilter.Error => entry.Level == EditorLogLevel.Error,
        EditorLogFilter.Build => entry.Source == EditorLogSource.Build,
        EditorLogFilter.Task => entry.Source == EditorLogSource.Task,
        EditorLogFilter.Input => entry.Source == EditorLogSource.Input,
        EditorLogFilter.Render => entry.Source == EditorLogSource.Render,
        _ => true
    };
}
