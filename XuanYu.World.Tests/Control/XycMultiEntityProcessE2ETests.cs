using System.Diagnostics; using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Editor.UI.XycControl;
namespace XuanYu.World.Tests.Xyc; [Collection("XYC Windows process runtime")] public sealed partial class XycMultiEntityProcessE2ETests {
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    [Fact]
    public async Task Silent_windows_editor_controls_two_entities_through_cli_process() {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var configuration = Directory.GetParent(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar))!.Name;
        var cli = Path.Combine(root, $"XuanYu.Editor.Control.Cli/bin/{configuration}/net10.0/XuanYu.Editor.Control.Cli.exe"); var app = Path.Combine(root, $"XuanYu.Editor.App/bin/{configuration}/net10.0/XuanYu.Editor.App.exe");
        Assert.True(File.Exists(cli), $"Missing CLI: {cli}"); Assert.True(File.Exists(app), $"Missing App: {app}");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(120)); await ReadOnlySession(cli, app, timeout.Token);
        var sid = "xyc-p2b1-" + Guid.NewGuid().ToString("N"); var (p, stderr) = Start(cli, app, sid, true);
        try
        {
            var runtime = await Send(p, sid, "runtime.info", new { }, timeout.Token);
            Ok(runtime); var host = runtime.GetProperty("observedState").GetProperty("silentHost");
            Assert.True(host.GetProperty("surfaceAndSwapchainAttached").GetBoolean());
            Assert.False(host.GetProperty("visible").GetBoolean()); Assert.False(host.GetProperty("wsVisibleStyle").GetBoolean());
            Assert.False(host.GetProperty("ownProcessForeground").GetBoolean());
            var scene = await Send(p, sid, "scene.inspect", new { }, timeout.Token); Ok(scene);
            var sidValue = scene.GetProperty("observedState").GetProperty("sceneId").GetString()!; var rev = scene.GetProperty("observedState").GetProperty("sceneRevision").GetInt64();
            var aState = State(await Send(p, sid, "scene.entity.create-test-cube", new { sceneId = sidValue, expectedRevision = rev }, timeout.Token)); var a = aState.SessionEntityIds![0];
            var bState = State(await Send(p, sid, "scene.entity.create-test-cube", new { sceneId = sidValue, expectedRevision = aState.SceneRevision }, timeout.Token)); var b = bState.SessionEntityIds![1];
            var movedA = State(await Send(p, sid, "scene.entity.set-test-position", Args(sidValue, bState, a, -1, 0, 0), timeout.Token)); var movedB = State(await Send(p, sid, "scene.entity.set-test-position", Args(sidValue, movedA, b, 1, 0, 0), timeout.Token));
            Assert.Equal(b, movedB.UndoTop!.EntityId); var wrong = await Send(p, sid, "scene.history.undo", new { sceneId = sidValue, expectedRevision = movedB.SceneRevision, entityId = a }, timeout.Token);
            Assert.Equal("HISTORY_CONFLICT", wrong.GetProperty("errorCode").GetString());
            var unchanged = State(await Send(p, sid, "scene.test.inspect", new { }, timeout.Token)); Assert.Equal(movedB.UndoTop, unchanged.UndoTop);
            Assert.Equal(movedB.UndoCount, unchanged.UndoCount); Assert.Equal(movedB.RedoCount, unchanged.RedoCount);
            var undoB = State(await Send(p, sid, "scene.history.undo", new { sceneId = sidValue, expectedRevision = movedB.SceneRevision, entityId = b }, timeout.Token)); Assert.Equal(new Vector3(-1, 0, 0), Entity(await Send(p, sid, "scene.entity.inspect", new { entityId = a }, timeout.Token)).Transform.Position); Assert.Equal(new Vector3(0, 0, 0), Entity(await Send(p, sid, "scene.entity.inspect", new { entityId = b }, timeout.Token)).Transform.Position);
            var undoA = State(await Send(p, sid, "scene.history.undo", new { sceneId = sidValue, expectedRevision = undoB.SceneRevision, entityId = a }, timeout.Token)); Assert.Equal(new Vector3(0, 0, 0), Entity(await Send(p, sid, "scene.entity.inspect", new { entityId = a }, timeout.Token)).Transform.Position);
            var redoA = State(await Send(p, sid, "scene.history.redo", new { sceneId = sidValue, expectedRevision = undoA.SceneRevision, entityId = a }, timeout.Token)); Assert.Equal(new Vector3(-1, 0, 0), Entity(await Send(p, sid, "scene.entity.inspect", new { entityId = a }, timeout.Token)).Transform.Position); var final = State(await Send(p, sid, "scene.history.redo", new { sceneId = sidValue, expectedRevision = redoA.SceneRevision, entityId = b }, timeout.Token));
            Assert.Equal(new[] { a, b }, final.SessionEntityIds); Assert.Contains(a, final.EntityIds); Assert.Contains(b, final.EntityIds); Assert.Contains(a, final.HierarchyEntityIds); Assert.Contains(b, final.HierarchyEntityIds);
            Assert.Contains(a, final.RenderEntityIds); Assert.Contains(b, final.RenderEntityIds);
            Assert.Equal(b, final.InspectorEntityId);
            Assert.Contains("X 1", Assert.Single(final.InspectorFields, x => x.Label == "位置").Value);
            var list = await Send(p, sid, "scene.entity.list", new { }, timeout.Token);
            Assert.Equal(new[] { a, b }, list.GetProperty("observedState").EnumerateArray()
                .Select(x => x.GetProperty("entityId").GetString()).Order().ToArray());
            Assert.Equal(new Vector3(-1, 0, 0), Entity(await Send(p, sid, "scene.entity.inspect", new { entityId = a }, timeout.Token)).Transform.Position);
            Assert.Equal(new Vector3(1, 0, 0), Entity(await Send(p, sid, "scene.entity.inspect", new { entityId = b }, timeout.Token)).Transform.Position);
            await CaptureHiddenFrame(p, sid, timeout.Token);
            await Close(p, sid, stderr, timeout.Token);
        }
        finally { if (!p.HasExited) p.Kill(entireProcessTree: true); }
    }
    static async Task ReadOnlySession(string cli, string app, CancellationToken token) {
        var sid = "xyc-p2b1-ro-" + Guid.NewGuid().ToString("N"); var (p, stderr) = Start(cli, app, sid, false);
        try
        {
            Ok(await Send(p, sid, "runtime.info", new { }, token));
            var scene = await Send(p, sid, "scene.inspect", new { }, token); Ok(scene);
            var state = scene.GetProperty("observedState");
            var write = await Send(p, sid, "scene.entity.create-test-cube", new { sceneId = state.GetProperty("sceneId").GetString(), expectedRevision = state.GetProperty("sceneRevision").GetInt64() }, token);
            Assert.Equal("UNAUTHORIZED", write.GetProperty("errorCode").GetString()); await Close(p, sid, stderr, token);
        }
        finally { if (!p.HasExited) p.Kill(entireProcessTree: true); }
    }
    static async Task CaptureHiddenFrame(Process p, string sid, CancellationToken token) {
        var camera = await Send(p, sid, "camera.inspect", new { }, token); Ok(camera);
        var rev = camera.GetProperty("observedState").GetProperty("revision").GetInt64();
        Assert.True(rev > 0);
        Ok(await Send(p, sid, "capture.arm", new { cameraRevision = rev }, token));
        try
        {
            var timer = Stopwatch.StartNew();
            while (timer.Elapsed < TimeSpan.FromSeconds(15))
            {
                var runtime = await Send(p, sid, "runtime.info", new { }, token);
                var host = runtime.GetProperty("observedState").GetProperty("silentHost");
                Assert.False(host.GetProperty("visible").GetBoolean()); Assert.False(host.GetProperty("ownProcessForeground").GetBoolean());
                if (host.GetProperty("readbackCaptured").GetBoolean())
                { var path = host.GetProperty("readbackPath").GetString()!; Assert.True(File.Exists(path)); Assert.Equal(host.GetProperty("readbackSha256").GetString()!.ToUpperInvariant(), Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)))); return; }
                await Task.Delay(150, token);
            }
            throw new TimeoutException("Hidden Vulkan GPU readback did not complete within 15 seconds.");
        }
        finally { Ok(await Send(p, sid, "capture.release", new { }, token)); }
    }
}
