using System.Diagnostics;
using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

if (args.Length < 2 || args[0] != "--app")
{
    Console.Error.WriteLine("用法：XuanYu.Editor.Control.Cli --app <XuanYu.Editor.App.exe> [--session-id <id>] [--diagnostic-write] [--test-dem <n24e121.hgt>] [--timeout-seconds <seconds>]");
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
var sessionId = Value(args, "--session-id") ?? Guid.NewGuid().ToString("N");
var timeoutSeconds = int.TryParse(Value(args, "--timeout-seconds"), out var seconds)
    ? seconds : 120;
if (timeoutSeconds is < 1 or > 3600) return 2;
var demPath = Value(args, "--test-dem");
if (demPath is not null && !args.Contains("--diagnostic-write"))
{
    Console.Error.WriteLine("--test-dem 需要显式 --diagnostic-write 授权。");
    return 2;
}
try
{
    if (demPath is not null)
        demPath = XycTestDemPolicy.Resolve(Path.GetDirectoryName(appPath)!, demPath);
}
catch (Exception error) when (error is UnauthorizedAccessException or IOException)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}
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
if (args.Contains("--diagnostic-write")) start.ArgumentList.Add("--xyc-diagnostic-write");
if (demPath is not null)
{
    start.ArgumentList.Add("--xyc-test-dem");
    start.ArgumentList.Add(demPath);
}
using var process = Process.Start(start);
if (process is null) return 3;
Console.Error.WriteLine($"XYC sessionId={sessionId}; childPid={process.Id}");
var output = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var pump = XycCliProcessBridge.PumpOutputAsync(process, output, closed);
var input = Task.Run(() => XycCliProcessBridge.PumpInputAsync(Console.In, process.StandardInput));
var exited = process.WaitForExitAsync();
var deadline = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
var trigger = await Task.WhenAny(input, closed.Task, exited, deadline);
if (trigger == deadline) return await XycCliProcessBridge.KillChildAsync(process, exited);
process.StandardInput.Close();
if (await Task.WhenAny(pump, deadline) == deadline)
    return await XycCliProcessBridge.KillChildAsync(process, exited);
await pump;
if (await Task.WhenAny(exited, deadline) == deadline)
    return await XycCliProcessBridge.KillChildAsync(process, exited);
await exited;
return process.ExitCode;

static string? Value(string[] values, string key)
{
    var index = Array.IndexOf(values, key);
    return index >= 0 && index + 1 < values.Length ? values[index + 1] : null;
}
