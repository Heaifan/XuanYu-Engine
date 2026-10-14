namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    internal IReadOnlyList<LogEntry> DiagnosticLogEntries => _logBuffer.All;
}
