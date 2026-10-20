using System.Reflection;
using XuanYu.Core.Diagnostics;
using XuanYu.Editor.UI.XycControl;

if (args.Length < 2 || args[0] != "--app")
{
    Console.Error.WriteLine("用法：XuanYu.Editor.Control.Cli --app <XuanYu.Editor.App.exe> [--session-id <id>] [--xyc-silent | --xyc-ui] [--evidence-dir <path>] [--diagnostic-write] [--test-dem <n24e121.hgt>] [--timeout-seconds <seconds>]");
    return 2;
}

var appPath = Path.GetFullPath(args[1]);
var appAssembly = Path.ChangeExtension(appPath, ".dll");
if (Path.GetFileName(appPath) != "XuanYu.Editor.App.exe" || !File.Exists(appAssembly) ||
    AssemblyName.GetAssemblyName(appAssembly).Name != "XuanYu.Editor.App")
{
    Console.Error.WriteLine("目标必须是随附的 XuanYu.Editor.App 启动器。");
    return 2;
}

var sessionId = Value(args, "--session-id") ?? Guid.NewGuid().ToString("N");
var timeout = int.TryParse(Value(args, "--timeout-seconds"), out var seconds) ? seconds : 120;
if (timeout is < 1 or > 3600) return 2;
var uiHost = args.Contains("--xyc-ui");
if (uiHost && args.Contains("--xyc-silent")) return 2;
var diagnosticsRoot = Environment.GetEnvironmentVariable("XYE_DIAGNOSTICS_DIRECTORY");
var defaultEvidenceDirectory = string.IsNullOrWhiteSpace(diagnosticsRoot)
    ? Path.Combine(Environment.CurrentDirectory, "artifacts", "diagnostics", "xyc-ui-v0.1-p0", sessionId)
    : Path.Combine(diagnosticsRoot, "xyc-ui", sessionId);
var evidenceDirectory = uiHost
    ? Path.GetFullPath(Value(args, "--evidence-dir") ?? defaultEvidenceDirectory)
    : null;
if (uiHost && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("XYE_TASK_RUN_ID")))
    evidenceDirectory = XyeWorkspacePaths.EnsureInsideWorkspace(evidenceDirectory!);
if (evidenceDirectory is not null) Directory.CreateDirectory(evidenceDirectory);
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

return await XycCliSessionRunner.RunAsync(appPath, sessionId, timeout, demPath,
    diagnosticWrite: args.Contains("--diagnostic-write"), silent: args.Contains("--xyc-silent"),
    uiHost: uiHost, evidenceDirectory: evidenceDirectory);

static string? Value(string[] values, string key)
{
    var index = Array.IndexOf(values, key);
    return index >= 0 && index + 1 < values.Length ? values[index + 1] : null;
}
