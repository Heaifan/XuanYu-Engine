using System.Diagnostics;
using System.Text.Json;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycProcessE2ETests
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Cli_controls_real_editor_camera_and_releases_instance()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var cli = Path.Combine(root, "XuanYu.Editor.Control.Cli/bin/Debug/net10.0/XuanYu.Editor.Control.Cli.exe");
        var app = Path.Combine(root, "XuanYu.Editor.App/bin/Debug/net10.0/XuanYu.Editor.App.exe");
        Assert.True(File.Exists(cli), $"Missing CLI: {cli}");
        Assert.True(File.Exists(app), $"Missing Editor App: {app}");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var start = new ProcessStartInfo(cli)
        {
            UseShellExecute = false, RedirectStandardInput = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        start.ArgumentList.Add("--app"); start.ArgumentList.Add(app);
        start.ArgumentList.Add("--session-id"); start.ArgumentList.Add("xyc-process-test");
        start.ArgumentList.Add("--diagnostic-write");
        using var process = Process.Start(start)!;
        try
        {
        var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
        var runtime = await Send(process, "r1", "runtime.info", new { }, timeout.Token);
        Assert.True(runtime.GetProperty("success").GetBoolean());
        var scene = await Send(process, "r2", "scene.inspect", new { }, timeout.Token);
        Assert.True(scene.GetProperty("success").GetBoolean());
        var before = await Send(process, "r3", "camera.inspect", new { }, timeout.Token);
        var changed = await Send(process, "r4", "camera.set-test-pose", Pose(101), timeout.Token);
        Assert.True(changed.GetProperty("success").GetBoolean());
        var first = await Send(process, "r5", "camera.inspect", new { }, timeout.Token);
        Assert.Equal(101, first.GetProperty("observedState").GetProperty("position").GetProperty("x").GetDouble());
        var changedAgain = await Send(process, "r6", "camera.set-test-pose", Pose(404), timeout.Token);
        Assert.True(changedAgain.GetProperty("success").GetBoolean());
        Assert.True(changedAgain.GetProperty("cameraRevision").GetInt64() >
            first.GetProperty("cameraRevision").GetInt64());
        var after = await Send(process, "r7", "camera.inspect", new { }, timeout.Token);
        Assert.True(after.GetProperty("cameraRevision").GetInt64() >
            before.GetProperty("cameraRevision").GetInt64());
        var observed = after.GetProperty("observedState");
        var position = observed.GetProperty("position");
        Assert.Equal(404, position.GetProperty("x").GetDouble());
        Assert.Equal(202, position.GetProperty("y").GetDouble());
        Assert.Equal(303, position.GetProperty("z").GetDouble());
        Assert.Equal(1, observed.GetProperty("observationCenter").GetProperty("x").GetDouble());
        Assert.Equal(-1, observed.GetProperty("forward").GetProperty("z").GetDouble());
        var orbit = await Send(process, "r8", "camera.orbit",
            new { deltaX = 40d, deltaY = -20d }, timeout.Token);
        Assert.True(orbit.GetProperty("success").GetBoolean(),
            orbit.GetProperty("errorCode").GetString());
        var orbitAfter = orbit.GetProperty("observedState").GetProperty("after");
        Assert.True(orbitAfter.GetProperty("revision").GetInt64() >
            observed.GetProperty("revision").GetInt64());
        Assert.Equal(1, orbitAfter.GetProperty("observationCenter").GetProperty("x").GetDouble());
        var closed = await Send(process, "r9", "session.close", new { }, timeout.Token);
        Assert.True(closed.GetProperty("success").GetBoolean());
        await process.WaitForExitAsync(timeout.Token);
        Assert.Equal(0, process.ExitCode);
        _ = await stderr;
        }
        finally
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
        }
    }

    static object Pose(double x) => new { position = new { x, y = 202d, z = 303d },
        forward = new { x = 0d, y = 0d, z = -1d }, up = new { x = 0d, y = 1d, z = 0d },
        fovDegrees = 75d, nearPlane = 0.05, farPlane = 1000d,
        mode = "Perspective", orthographicScale = 0d,
        observationCenter = new { x = 1d, y = 2d, z = 3d }, revision = 0 };

    static async Task<JsonElement> Send(Process process, string id, string command,
        object args, CancellationToken token)
    {
        var request = new { schemaVersion = "XYC-API/1", requestId = id,
            sessionId = "xyc-process-test", command, args };
        await process.StandardInput.WriteLineAsync(JsonSerializer.Serialize(request, Json));
        var line = await process.StandardOutput.ReadLineAsync(token);
        Assert.NotNull(line);
        return JsonDocument.Parse(line!).RootElement.Clone();
    }
}
