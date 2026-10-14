using System.Diagnostics;
using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

if (args.Length is < 2 or > 4 || args[0] != "--app")
{
    Console.Error.WriteLine("用法：XuanYu.Editor.Control.Cli --app <XuanYu.Editor.App.exe> [--session-id <id>]");
    return 2;
}
var appPath = Path.GetFullPath(args[1]);
var appAssembly = Path.ChangeExtension(appPath, ".dll");
if (Path.GetFileName(appPath) != "XuanYu.Editor.App.exe" ||
    !File.Exists(appAssembly) ||
    System.Reflection.AssemblyName.GetAssemblyName(appAssembly).Name != "XuanYu.Editor.App")
{
    Console.Error.WriteLine("目标必须是随附的 XuanYu.Editor.App 启动器。");
    return 2;
}
var sessionId = args.Length == 4 && args[2] == "--session-id"
    ? args[3] : Guid.NewGuid().ToString("N");
var start = new ProcessStartInfo(appPath)
{
    UseShellExecute = false,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    RedirectStandardError = false
};
start.ArgumentList.Add("--xyc-stdio");
start.ArgumentList.Add("--xyc-session");
start.ArgumentList.Add(sessionId);
start.ArgumentList.Add("--xyc-diagnostic-write");
using var process = Process.Start(start);
if (process is null) return 3;
Console.Error.WriteLine($"XYC sessionId={sessionId}");
var output = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var pump = PumpOutputAsync(process, output, closed);
var input = PumpInputAsync(Console.In, process.StandardInput);
await Task.WhenAny(input, closed.Task);
process.StandardInput.Close();
await pump;
await process.WaitForExitAsync();
return process.ExitCode;

static async Task PumpInputAsync(TextReader input, StreamWriter childInput)
{
    while (await input.ReadLineAsync() is { } line)
        await childInput.WriteLineAsync(line);
}

static async Task PumpOutputAsync(Process process, TextWriter output,
    TaskCompletionSource closed)
{
    while (await process.StandardOutput.ReadLineAsync() is { } line)
    {
        await output.WriteLineAsync(line);
        using var json = JsonDocument.Parse(line);
        if (json.RootElement.TryGetProperty("observedState", out var state) &&
            state.TryGetProperty("closed", out var value) && value.GetBoolean())
            closed.TrySetResult();
    }
}
