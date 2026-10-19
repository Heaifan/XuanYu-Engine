using System.Diagnostics;
using System.Text.Json;

namespace XuanYu.World.Tests.Xyc;

[Collection("XYC Windows process runtime")]
public sealed class XycCliGuardE2ETests
{
    [Fact]
    [Trait("Platform", "Windows")]
    public async Task Default_cli_session_denies_camera_write()
    {
        using var process = Start("--timeout-seconds", "30");
        var stderr = process.StandardError.ReadToEndAsync();
        try
        {
            var request = new { schemaVersion = "XYC-API/1", requestId = "read-only",
                sessionId = "xyc-read-only", command = "camera.set-test-pose", args = new { } };
            await process.StandardInput.WriteLineAsync(JsonSerializer.Serialize(request));
            var line = await process.StandardOutput.ReadLineAsync()
                .WaitAsync(TimeSpan.FromSeconds(20));
            using var response = JsonDocument.Parse(line!);
            Assert.Equal("UNAUTHORIZED", response.RootElement.GetProperty("errorCode").GetString());
            await process.StandardInput.WriteLineAsync(Request("close", "session.close"));
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(10));
            Assert.True(process.ExitCode == 0, await stderr);
        }
        finally { if (!process.HasExited) process.Kill(entireProcessTree: true); }
    }

    [Fact]
    [Trait("Platform", "Windows")]
    public async Task Timeout_exits_cli_and_kills_editor_process_tree()
    {
        using var process = Start("--timeout-seconds", "1");
        var stderr = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
        Assert.Equal(124, process.ExitCode);
        var childPid = int.Parse((await stderr).Split("childPid=")[1].Split(';')[0]);
        Assert.Throws<ArgumentException>(() => Process.GetProcessById(childPid));
    }

    static Process Start(params string[] options)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var cli = Path.Combine(root, "XuanYu.Editor.Control.Cli/bin/Debug/net10.0/XuanYu.Editor.Control.Cli.exe");
        var app = Path.Combine(root, "XuanYu.Editor.App/bin/Debug/net10.0/XuanYu.Editor.App.exe");
        var start = new ProcessStartInfo(cli) { UseShellExecute = false,
            RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true };
        start.ArgumentList.Add("--app"); start.ArgumentList.Add(app);
        start.ArgumentList.Add("--session-id"); start.ArgumentList.Add("xyc-read-only");
        foreach (var option in options) start.ArgumentList.Add(option);
        return Process.Start(start)!;
    }

    static string Request(string id, string command) => JsonSerializer.Serialize(new
    { schemaVersion = "XYC-API/1", requestId = id, sessionId = "xyc-read-only", command, args = new { } });
}
