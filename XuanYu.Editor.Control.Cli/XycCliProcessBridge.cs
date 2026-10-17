using System.Diagnostics;
using System.Text.Json;

internal static class XycCliProcessBridge
{
    public static async Task<int> KillChildAsync(Process process, Task exited, int exitCode = 124)
    {
        if (!process.HasExited) process.Kill(entireProcessTree: true);
        await exited.WaitAsync(TimeSpan.FromSeconds(5));
        return exitCode;
    }

    public static async Task PumpInputAsync(TextReader input, StreamWriter childInput)
    {
        while (await input.ReadLineAsync() is { } line)
            await childInput.WriteLineAsync(line);
    }

    public static async Task PumpOutputAsync(Process process, TextWriter output,
        TaskCompletionSource closed, bool silent = false)
    {
        while (await process.StandardOutput.ReadLineAsync() is { } line)
        {
            await output.WriteLineAsync(line);
            using var json = JsonDocument.Parse(line);
            if (silent) WriteRuntimeEvidence(json.RootElement);
            if (json.RootElement.TryGetProperty("observedState", out var state) &&
                state.ValueKind == JsonValueKind.Object &&
                state.TryGetProperty("closed", out var value) && value.GetBoolean())
                closed.TrySetResult();
        }
    }

    private static void WriteRuntimeEvidence(JsonElement root)
    {
        var fields = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        Collect(root, fields, depth: 0);
        if (fields.Count == 0) return;

        var evidence = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in fields)
            evidence[field.Key] = field.Value;
        Console.Error.WriteLine("XYC silent runtime evidence=" + JsonSerializer.Serialize(evidence));
    }

    private static void Collect(JsonElement source, IDictionary<string, JsonElement> destination,
        int depth)
    {
        if (depth > 4 || source.ValueKind != JsonValueKind.Object) return;
        foreach (var property in source.EnumerateObject())
        {
            var name = property.Name;
            if (IsEvidenceField(name) && (property.Value.ValueKind is JsonValueKind.String or
                JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null))
                destination[name] = property.Value.Clone();
            if (property.Value.ValueKind == JsonValueKind.Object)
                Collect(property.Value, destination, depth + 1);
        }
    }

    private static bool IsEvidenceField(string name)
    {
        string[] tokens = ["Build", "BuildIdentity", "Revision", "Frame", "Present", "Readback",
            "Extent", "Viewport", "WindowHandle", "Hwnd", "ImageHash", "Sha256",
            "Swapchain", "Surface", "ImageIndex", "Device", "Width", "Height",
            "EvidencePath", "Capability", "Isolation", "ProcessId", "Visible",
            "Attached", "Failure"];
        return tokens.Any(token => name.Contains(token, StringComparison.OrdinalIgnoreCase));
    }
}
