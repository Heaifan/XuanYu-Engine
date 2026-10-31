namespace XuanYu.Editor.UI;

public sealed record EditorLogFilterState(
    bool Info,
    bool Warning,
    bool Error,
    EditorLogSource? Source = null,
    string SearchText = "")
{
    public bool IsAllSelected => Info && Warning && Error;

    public static EditorLogFilterState CreateDefault() => new(true, true, true);

    public EditorLogFilterState ToggleSeverity(EditorLogLevel level)
    {
        var next = level switch
        {
            EditorLogLevel.Info => this with { Info = !Info },
            EditorLogLevel.Warning => this with { Warning = !Warning },
            EditorLogLevel.Error => this with { Error = !Error },
            _ => this
        };
        return next;
    }

    public EditorLogFilterState SetAll(bool value) => this with { Info = value, Warning = value, Error = value };

    public bool Allows(LogEntry entry)
    {
        var levelAllowed = entry.Level switch
        {
            EditorLogLevel.Info or EditorLogLevel.Debug or EditorLogLevel.Trace => Info,
            EditorLogLevel.Warning => Warning,
            EditorLogLevel.Error => Error,
            _ => true
        };
        var sourceAllowed = Source is null || Source == entry.Source;
        var searchAllowed = string.IsNullOrWhiteSpace(SearchText)
            || entry.Message.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || entry.Detail.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        return levelAllowed && sourceAllowed && searchAllowed;
    }
}
