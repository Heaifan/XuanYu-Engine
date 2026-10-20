using System.Text.Json;

namespace XuanYu.Render.Abstractions.Diagnostics;

public static class RendererEventReader
{
    public static IReadOnlyList<RendererEvent> Read(
        string path, string? eventCode = null, string? runId = null, string? sessionId = null)
    {
        if (!File.Exists(path)) return [];
        var result = new List<RendererEvent>();
        foreach (var line in File.ReadLines(path))
        {
            var item = TryRead(line);
            if (item is not null && Matches(item, eventCode, runId, sessionId)) result.Add(item);
        }
        return result;
    }

    static RendererEvent? TryRead(string line)
    {
        try { return JsonSerializer.Deserialize<RendererEvent>(line, RendererEventJson.Options); }
        catch (JsonException) { return null; }
    }

    static bool Matches(RendererEvent item, string? code, string? run, string? session) =>
        (string.IsNullOrWhiteSpace(code) || item.EventCode == code) &&
        (string.IsNullOrWhiteSpace(run) || item.RunId == run) &&
        (string.IsNullOrWhiteSpace(session) || item.SessionId == session);
}
