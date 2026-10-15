using System.Diagnostics;

namespace XuanYu.Core.Diagnostics;

public static partial class ViewportProbe
{
    static readonly SemaphoreSlim PresentedSignal = new(0);
    static readonly ManualResetEventSlim CaptureRelease = new(true);
    static readonly object FrameGate = new();
    static long _nextPresentedFrame;
    static ViewportPresentedFrame? _lastPresented;
    public static ViewportPresentedFrame? LastPresented => Volatile.Read(ref _lastPresented);
    public static long LastPresentedFrame => LastPresented?.FrameId ?? 0;
    public static long LastPresentedCameraRevision => LastPresented?.CameraRevision ?? -1;
    public static long NextPresentedFrameId() => Interlocked.Increment(ref _nextPresentedFrame);

    public static bool ArmFrameCapture(long cameraRevision = 0)
    {
        if (!TerrainReplayOptions.Enabled) return false;
        CaptureRelease.Reset();
        lock (FrameGate)
        {
            _captureAfterFrame = LastPresentedFrame;
            _captureCameraRevision = cameraRevision;
        }
        Interlocked.Exchange(ref _captureArmed, 1);
        return true;
    }

    public static void ReleaseCapturedFrame() => CaptureRelease.Set();

    static int _captureArmed;
    static long _captureAfterFrame;
    static long _captureCameraRevision;

    public static void FrameSubmitted(long frameId, uint imageIndex, long cameraRevision) =>
        Log("present", $"[FRAME-SUBMITTED] GpuFrameId={frameId};" +
            $"ImageIndex={imageIndex};CameraRevision={cameraRevision};" +
            $"RenderSnapshotCameraRevision={cameraRevision};QueueSubmit=SUCCESS");

    public static void FramePresented(long frameId, uint imageIndex, long cameraRevision)
    {
        bool holdForCapture;
        lock (FrameGate)
        {
            holdForCapture = Volatile.Read(ref _captureArmed) == 1 &&
                frameId > _captureAfterFrame &&
                (_captureCameraRevision == 0 || cameraRevision == _captureCameraRevision);
            if (holdForCapture) Interlocked.Exchange(ref _captureArmed, 0);
            Volatile.Write(ref _lastPresented,
                new ViewportPresentedFrame(frameId, imageIndex, cameraRevision, holdForCapture));
        }
        Log("present", $"[FRAME-PRESENTED] GpuFrameId={frameId};" +
            $"ImageIndex={imageIndex};CameraRevision={cameraRevision};" +
            $"RenderSnapshotCameraRevision={cameraRevision};" +
            $"QueuePresent=SUCCESS;CaptureHold={holdForCapture}");
        PresentedSignal.Release();
        if (holdForCapture && !CaptureRelease.Wait(TimeSpan.FromSeconds(30)))
            Log("present", $"[FRAME-CAPTURE-TIMEOUT] GpuFrameId={frameId}");
    }

    public static async Task<long> WaitForPresentedFrameAsync(long afterFrameId,
        TimeSpan timeout)
    {
        var timer = Stopwatch.StartNew();
        while (LastPresentedFrame <= afterFrameId)
        {
            var remaining = timeout - timer.Elapsed;
            if (remaining <= TimeSpan.Zero || !await PresentedSignal.WaitAsync(remaining))
                throw new TimeoutException($"等待 Vulkan FrameId > {afterFrameId} 超时。");
        }
        return LastPresentedFrame;
    }
}
