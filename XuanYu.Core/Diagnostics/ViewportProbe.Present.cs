using System.Diagnostics;

namespace XuanYu.Core.Diagnostics;

public static partial class ViewportProbe
{
    static readonly SemaphoreSlim PresentedSignal = new(0);
    static readonly ManualResetEventSlim CaptureRelease = new(true);
    static readonly object FrameGate = new();
    static long _nextPresentedFrame;
    static long _lastPresentedFrame;
    public static long LastPresentedFrame => Interlocked.Read(ref _lastPresentedFrame);

    public static long NextPresentedFrameId() => Interlocked.Increment(ref _nextPresentedFrame);

    public static void ArmFrameCapture()
    {
        if (!TerrainReplayOptions.Enabled) return;
        CaptureRelease.Reset();
        lock (FrameGate) _captureAfterFrame = _lastPresentedFrame;
        Interlocked.Exchange(ref _captureArmed, 1);
    }

    public static void ReleaseCapturedFrame() => CaptureRelease.Set();

    static int _captureArmed;
    static long _captureAfterFrame;

    public static void FrameSubmitted(long frameId, uint imageIndex) =>
        Log("present", $"[FRAME-SUBMITTED] GpuFrameId={frameId};" +
            $"ImageIndex={imageIndex};QueueSubmit=SUCCESS");

    public static void FramePresented(long frameId, uint imageIndex)
    {
        bool holdForCapture;
        lock (FrameGate)
        {
            Interlocked.Exchange(ref _lastPresentedFrame, frameId);
            holdForCapture = Interlocked.Exchange(ref _captureArmed, 0) == 1 &&
                frameId > _captureAfterFrame;
        }
        Log("present", $"[FRAME-PRESENTED] GpuFrameId={frameId};" +
            $"ImageIndex={imageIndex};QueuePresent=SUCCESS;CaptureHold={holdForCapture}");
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
