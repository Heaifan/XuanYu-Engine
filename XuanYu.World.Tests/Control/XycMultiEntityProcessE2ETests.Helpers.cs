using System.Diagnostics;
using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed partial class XycMultiEntityProcessE2ETests
{
    static object Args(string scene, XycSceneState state, string id, double x, double y, double z) =>
        new { sceneId = scene, expectedRevision = state.SceneRevision, entityId = id, position = new { x, y, z } };
    static XycSceneState State(JsonElement e) { Ok(e); return e.GetProperty("observedState").Deserialize<XycSceneState>(Json)!; }
    static XycEntity Entity(JsonElement e) { Ok(e); return e.GetProperty("observedState").Deserialize<XycEntity>(Json)!; }
    static void Ok(JsonElement e) => Assert.True(e.GetProperty("success").GetBoolean(), e.GetProperty("errorCode").GetString());
    static (Process, Task<string>) Start(string cli, string app, string sid, bool write) {
        var start = new ProcessStartInfo(cli) { UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true };
        start.ArgumentList.Add("--app"); start.ArgumentList.Add(app); start.ArgumentList.Add("--session-id"); start.ArgumentList.Add(sid);
        start.ArgumentList.Add("--xyc-silent"); start.ArgumentList.Add("--timeout-seconds"); start.ArgumentList.Add("120");
        if (write) start.ArgumentList.Add("--diagnostic-write");
        var p = Process.Start(start)!; return (p, p.StandardError.ReadToEndAsync());
    }
    static async Task<JsonElement> Send(Process p, string sid, string command, object args, CancellationToken token) {
        var request = new { schemaVersion = "XYC-API/1", requestId = Guid.NewGuid().ToString("N"), sessionId = sid, command, args };
        await p.StandardInput.WriteLineAsync(JsonSerializer.Serialize(request, Json));
        return JsonDocument.Parse((await p.StandardOutput.ReadLineAsync(token))!).RootElement.Clone();
    }
    static async Task Close(Process p, string sid, Task<string> stderr, CancellationToken token) {
        Ok(await Send(p, sid, "session.close", new { }, token)); await p.WaitForExitAsync(token);
        Assert.Equal(0, p.ExitCode); _ = await stderr;
    }
}
