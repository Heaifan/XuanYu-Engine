using System.Text.Json;
using Avalonia.Controls;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

internal static class TerrainNativeInputReplay
{
    public static bool Requested =>
        Environment.GetEnvironmentVariable("XYE_TERRAIN_NATIVE_INPUT_REPLAY") == "1";

    public static async Task RunAsync(UiVm vm, Window window, string output,
        string runId, long afterFrame)
    {
        var ready = Path.Combine(output, "native-input.ready.json");
        var command = Path.Combine(output, "native-input.next.json");
        File.WriteAllText(ready, JsonSerializer.Serialize(new
        {
            RunId = runId, ProcessId = Environment.ProcessId,
            WindowTitle = window.Title, Camera = vm.RenderSnapshot.CameraState,
            ObservationCenter = vm.ObservationCenter, Viewport = vm.CurrentViewport,
            ReadyAt = DateTimeOffset.UtcNow
        }, new JsonSerializerOptions { WriteIndented = true }));
        ViewportProbe.Log("native-input", $"[NATIVE-REPLAY-READY] RunId={runId};" +
            $"ProcessId={Environment.ProcessId};Window={window.Title}");
        var completed = 0;
        while (completed < 128 && !File.Exists(Path.Combine(output, "native-input.stop")))
        {
            if (!File.Exists(command)) { await Task.Delay(20); continue; }
            NativeInputCaptureCommand? request = null;
            for (var attempt = 0; attempt < 50 && request is null; attempt++)
            {
                try
                {
                    request = JsonSerializer.Deserialize<NativeInputCaptureCommand>(
                        await File.ReadAllTextAsync(command));
                }
                catch (IOException) when (attempt < 49) { await Task.Delay(20); }
            }
            if (request is null) throw new InvalidDataException("Native input command could not be read.");
            File.Delete(command);
            var nextFrame = ViewportProbe.LastPresentedFrame;
            ViewportProbe.ArmFrameCapture();
            File.WriteAllText(Path.Combine(output, request.Stage + ".armed"), "ARMED");
            try
            {
                var frame = await ViewportProbe.WaitForPresentedFrameAsync(
                    nextFrame, TimeSpan.FromSeconds(30));
                await TerrainReplayCapture.RequestAsync(vm, window, output, runId,
                    request.Stage, frame);
                File.WriteAllText(Path.Combine(output, request.Stage + ".complete"),
                    $"FrameId={frame};CameraRevision={vm.RenderSnapshot.CameraState.Revision}");
                completed++;
            }
            finally { ViewportProbe.ReleaseCapturedFrame(); }
        }
        ViewportProbe.Log("native-input", $"[NATIVE-REPLAY-COMPLETE] RunId={runId};Stages={completed}");
    }

    sealed record NativeInputCaptureCommand(string Stage);
}
