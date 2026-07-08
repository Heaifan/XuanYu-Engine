namespace XuanYu.Editor.UI;

public sealed class EditorLogBus(EditorLogBuffer buffer)
{
    public void Info(EditorLogSource source, EditorLogCategory category, string message, string detail = "", string contextId = "") =>
        Write(EditorLogLevel.Info, source, category, message, detail, contextId);

    public void Warning(EditorLogSource source, EditorLogCategory category, string message, string detail = "", string contextId = "") =>
        Write(EditorLogLevel.Warning, source, category, message, detail, contextId);

    public void Error(EditorLogSource source, EditorLogCategory category, string message, string detail = "", string contextId = "") =>
        Write(EditorLogLevel.Error, source, category, message, detail, contextId);

    void Write(EditorLogLevel level, EditorLogSource source, EditorLogCategory category, string message, string detail, string contextId)
    {
        var time = DateTime.Now.ToString("HH:mm:ss");
        var correlationId = $"{source}-{DateTime.Now:HHmmss}";
        var entry = new LogEntry(time, level, source, category, message, detail, contextId, correlationId);
        buffer.Add(entry);
    }
}
