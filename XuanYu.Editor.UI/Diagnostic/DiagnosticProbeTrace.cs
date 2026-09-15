namespace XuanYu.Editor.UI;

internal static class DiagnosticProbeTrace
{
    public static bool HostLoaded { get; private set; }
    public static bool HandlersAttached { get; private set; }
    public static int PointerMovedCount { get; private set; }
    public static string LastPointerSourceType { get; private set; } = "N/A";
    public static int ProbeHoverCount { get; private set; }
    public static int ResolverResultCount { get; private set; }
    public static int TranslatePointAttemptCount { get; private set; }
    public static int TranslatePointSuccessCount { get; private set; }
    public static int RenderCount { get; private set; }

    public static void Reset() => (HostLoaded, HandlersAttached, PointerMovedCount,
        ProbeHoverCount, ResolverResultCount, TranslatePointAttemptCount,
        TranslatePointSuccessCount, RenderCount, LastPointerSourceType) =
        (false, false, 0, 0, 0, 0, 0, 0, "N/A");

    public static void MarkHostLoaded() => HostLoaded = true;
    public static void MarkHandlers(bool attached) => HandlersAttached = attached;
    public static void MarkPointer(object? source)
    {
        PointerMovedCount++;
        LastPointerSourceType = source?.GetType().Name ?? "NULL";
    }

    public static void MarkProbeHover() => ProbeHoverCount++;
    public static void MarkResolverResult() => ResolverResultCount++;
    public static void MarkTranslateAttempt() => TranslatePointAttemptCount++;
    public static void MarkTranslateSuccess() => TranslatePointSuccessCount++;
    public static void MarkRender() => RenderCount++;

    public static string CardText(bool diagnostic, bool probe) =>
        $"PROBE TRACE\nHostLoaded       {HostLoaded}\nHandlersAttached {HandlersAttached}\n\n" +
        $"Diagnostic       {diagnostic}\nProbe            {probe}\n\n" +
        $"PointerMoved     {PointerMovedCount}\nSource           {LastPointerSourceType}\n" +
        $"ProbeHover       {ProbeHoverCount}\nResolver         {ResolverResultCount}\n" +
        $"Translate        {TranslatePointAttemptCount} / {TranslatePointSuccessCount}\n" +
        $"Render           {RenderCount}";
}
