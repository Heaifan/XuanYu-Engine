using System.Security.Cryptography;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

internal sealed class SilentCaptureEvidence
{
    long _lastHashedFrame;
    public string? Path { get; private set; }
    public string? Sha256 { get; private set; }
    public bool Captured => Path is not null;

    public void Update(ViewportPresentedFrame? frame)
    {
        if (frame is not { CaptureHeld: true } || frame.FrameId == _lastHashedFrame) return;
        _lastHashedFrame = frame.FrameId;
        var directory = XyeDiagnosticPaths.ResolveProbeDirectory(
            $"viewport-{Environment.ProcessId}");
        var path = System.IO.Path.Combine(directory,
            $"gpu-frame-{frame.FrameId:D8}-camera-{frame.CameraRevision:D8}.png");
        if (!File.Exists(path)) return;
        Path = path;
        Sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
    }
}
