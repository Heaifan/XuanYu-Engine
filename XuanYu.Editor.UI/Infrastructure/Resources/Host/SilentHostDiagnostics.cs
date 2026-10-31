namespace XuanYu.Editor.UI;

public sealed record SilentHostDiagnostics(string Mode, long Hwnd, bool HwndExists,
    bool Visible, bool WsVisibleStyle, int ClientWidth, int ClientHeight,
    int RequestedWidth, int RequestedHeight, string? AttachFailure,
    bool SurfaceAndSwapchainAttached, bool PresentSucceeded, bool ReadbackCaptured,
    long FrameCounter, long? PresentedFrameId, long? PresentedCameraRevision,
    uint? PresentedImageIndex, bool? CaptureHeld, bool ProbeEnabled,
    bool OwnProcessForeground, string? ReadbackPath, string? ReadbackSha256,
    string BuildIdentity);
