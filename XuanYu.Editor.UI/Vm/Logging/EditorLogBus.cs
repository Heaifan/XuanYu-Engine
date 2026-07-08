using System.IO;

namespace XuanYu.Editor.UI;

public sealed class EditorLogBus(EditorLogBuffer buffer)
{
    readonly string _logDir = Path.Combine(Environment.CurrentDirectory, "logs");
    readonly string _logPath = Path.Combine(Environment.CurrentDirectory, "logs", "editor-session-latest.log");
    bool _sessionMarked;

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
        AppendToFile(entry);
    }

    void AppendToFile(LogEntry e)
    {
        try
        {
            if (!_sessionMarked)
            {
                Directory.CreateDirectory(_logDir);
                File.WriteAllText(_logPath, $"=== 会话开始 {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
                _sessionMarked = true;
            }
            var line = string.Join("\t", e.Time, e.LevelText, e.SourceText, e.CategoryText, e.Message, e.DetailText);
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }
        catch { /* 诊断安全：落盘失败不阻塞 UI 日志 */ }
    }
}
