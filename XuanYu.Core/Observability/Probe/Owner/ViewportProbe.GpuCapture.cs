using System.Security.Cryptography;
using System.Text.Json;

namespace XuanYu.Core.Diagnostics;

public static partial class ViewportProbe
{
    static string _gpuDeviceName = "UNKNOWN";
    public static void LogGpuReadbackCapability(string device, string format,
        bool transferSrcEnabled)
    {
        _gpuDeviceName = device;
        Log("present", $"[GPU-READBACK-CAPABILITY] Device={device};Format={format};" +
            $"TransferSrcEnabled={transferSrcEnabled}");
    }

    public static bool IsCaptureTargetFrame(long frameId, long cameraRevision)
    {
        lock (FrameGate)
            return Volatile.Read(ref _captureArmed) == 1 && frameId > _captureAfterFrame &&
                (_captureCameraRevision == 0 || cameraRevision == _captureCameraRevision);
    }

    internal static byte[] EncodeGpuPixels(int width, int height, string format,
        ReadOnlySpan<byte> pixels) => format switch
    {
        "B8G8R8A8Unorm" or "B8G8R8A8Srgb" =>
            VulkanPngEncoder.EncodeBgra(width, height, pixels),
        "R8G8B8A8Unorm" or "R8G8B8A8Srgb" =>
            VulkanPngEncoder.EncodeRgba(width, height, pixels),
        _ => throw new NotSupportedException($"Unsupported GPU pixel format: {format}")
    };

    public static string WriteGpuCapture(long frameId, uint imageIndex, long cameraRevision,
        int width, int height, string format, ReadOnlySpan<byte> pixels)
    {
        var png = EncodeGpuPixels(width, height, format, pixels);
        var path = Path.Combine(_directory, $"gpu-frame-{frameId:D8}-camera-{cameraRevision:D8}.png");
        var hash = Convert.ToHexString(SHA256.HashData(png));
        File.WriteAllBytes(path, png);
        File.WriteAllText(Path.ChangeExtension(path, ".json"), JsonSerializer.Serialize(new
        {
            Source = "VulkanSwapchainImage-GPU-Readback", Device = _gpuDeviceName,
            Build = ViewportProbeIdentity.Current(),
            FrameId = frameId, CameraRevision = cameraRevision, ImageIndex = imageIndex,
            Width = width, Height = height, Format = format, Sha256 = hash
        }, new JsonSerializerOptions { WriteIndented = true }));
        Log("present", $"[GPU-PNG] Path={path};FrameId={frameId};" +
            $"CameraRevision={cameraRevision};Format={format};SHA256={hash}");
        return path;
    }
}
