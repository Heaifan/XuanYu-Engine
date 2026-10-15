using System.Diagnostics;
using System.Text.Json;

internal static class XycCliProcessBridge
{
    public static async Task<int> KillChildAsync(Process process, Task exited)
    {
        if (!process.HasExited) process.Kill(entireProcessTree: true);
        await exited.WaitAsync(TimeSpan.FromSeconds(5));
        return 124;
    }

    public static async Task PumpInputAsync(TextReader input, StreamWriter childInput)
    {
        while (await input.ReadLineAsync() is { } line)
            await childInput.WriteLineAsync(line);
    }

    public static async Task PumpOutputAsync(Process process, TextWriter output,
        TaskCompletionSource closed)
    {
        while (await process.StandardOutput.ReadLineAsync() is { } line)
        {
            await output.WriteLineAsync(line);
            using var json = JsonDocument.Parse(line);
            if (json.RootElement.TryGetProperty("observedState", out var state) &&
                state.ValueKind == JsonValueKind.Object &&
                state.TryGetProperty("closed", out var value) && value.GetBoolean())
                closed.TrySetResult();
        }
    }
}
