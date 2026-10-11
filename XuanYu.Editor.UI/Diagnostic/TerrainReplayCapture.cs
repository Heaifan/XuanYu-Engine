using System.Text.Json;
using System.Diagnostics;
using Avalonia.Controls;
using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

internal static class TerrainReplayCapture
{
    public static async Task<long> CaptureNextAsync(UiVm vm, Window window,
        string output, string runId, string stage, long afterFrame)
    {
        ViewportProbe.ArmFrameCapture();
        var frame = await ViewportProbe.WaitForPresentedFrameAsync(
            afterFrame, TimeSpan.FromSeconds(15));
        try
        {
            await RequestAsync(vm, window, output, runId, stage, frame);
        }
        finally
        {
            ViewportProbe.ReleaseCapturedFrame();
        }
        return frame;
    }

    public static async Task RequestAsync(UiVm vm, Window window, string output,
        string runId, string stage, long frame)
    {
        await Task.Delay(80);
        var camera = vm.RenderSnapshot.CameraState;
        var farOverride = TerrainReplayOptions.FarPlaneOverride;
        var renderCamera = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayOptions.Variant, farOverride);
        var viewport = vm.CurrentViewport;
        var projection = ViewProjectionState.Create(camera, viewport);
        var terrain = vm.RenderProjection.Projection.TerrainResources.FirstOrDefault();
        var image = Path.Combine(output, $"{stage}.png");
        var requestPath = Path.Combine(output, $"{stage}.capture.request.json");
        var ackPath = requestPath + ".ack";
        var request = new TerrainReplayCaptureRequest(runId, stage, frame,
            ViewportProbe.CurrentFrameId, ViewportProbe.CurrentNativeInputId,
            TerrainReplayOptions.Variant.ToString(),
            farOverride, Process.GetCurrentProcess().Id,
            window.Title ?? string.Empty, image, ViewportProbeIdentity.Current(),
            Environment.GetEnvironmentVariable("XYE_PROBE_SOURCE_FINGERPRINT") ?? "UNKNOWN",
            Environment.GetEnvironmentVariable("XYE_PROBE_PUBLISH_FINGERPRINT") ?? "UNKNOWN",
            camera.Position, camera.Forward, camera.VerticalFovDegrees,
            camera.NearPlane, camera.FarPlane,
            renderCamera.NearPlane, renderCamera.FarPlane,
            camera.Revision, vm.ObservationCenter, projection.RenderOrigin,
            terrain?.TerrainId ?? "NONE", terrain?.Revision ?? -1,
            viewport.PhysicalWidth, viewport.PhysicalHeight);
        await File.WriteAllTextAsync(requestPath,
            JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true }));
        ViewportProbe.Log("replay", $"[CAPTURE-REQUEST] Stage={stage};" +
            $"FrameId={frame};Path={requestPath}");
        for (var attempt = 0; attempt < 300 && !File.Exists(ackPath); attempt++)
            await Task.Delay(100);
        if (!File.Exists(ackPath)) throw new TimeoutException($"等待窗口截图确认超时：{stage}");
        if (await File.ReadAllTextAsync(ackPath) != "CAPTURED")
            throw new IOException($"Windows 截图失败：{await File.ReadAllTextAsync(ackPath)}");
        ViewportProbe.Log("replay", $"[CAPTURE-COMPLETE] Stage={stage};" +
            $"FrameId={frame};Image={image}");
    }
}

internal sealed record TerrainReplayCaptureRequest(
    string RunId, string Stage, long FrameId, long ViewportFrameId,
    long NativeInputEventId,
    string Variant, double? FarPlaneOverride, int ProcessId,
    string WindowTitle, string ScreenshotPath, string BuildIdentity,
    string SourceTreeFingerprint, string PublishedOutputFingerprint,
    object CameraPosition, object CameraForward, double Fov, double NearPlane,
    double FarPlane, double EffectiveNearPlane, double EffectiveFarPlane,
    long CameraRevision, object ObservationCenter,
    object RenderOrigin, string TerrainId, int TerrainRevision,
    int PhysicalWidth, int PhysicalHeight);
