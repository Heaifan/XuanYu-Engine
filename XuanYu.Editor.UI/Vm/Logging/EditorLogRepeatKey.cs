namespace XuanYu.Editor.UI;

public readonly record struct EditorLogRepeatKey(
    EditorLogLevel Level,
    EditorLogSource Source,
    EditorLogCategory Category,
    string Message,
    string ContextId)
{
    public static EditorLogRepeatKey From(LogEntry entry) =>
        new(entry.Level, entry.Source, entry.Category, entry.Message, entry.ContextId);
}
