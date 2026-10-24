namespace XuanYu.Core.Diagnostics;

public sealed record ViewportPresentedFrame(long FrameId, uint ImageIndex,
    long CameraRevision, bool CaptureHeld);
