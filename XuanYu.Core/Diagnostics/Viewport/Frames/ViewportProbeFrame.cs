using System.Numerics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Diagnostics;

public static partial class ViewportProbe
{
    static long _lastGpuFrame = -1;
    static string? _lastGpuHash;

    public static void CameraSnapshot(string stage, CameraState camera,
        Vector3d pivot, ViewProjectionState? state = null)
    {
        if (!Enabled) return;
        var origin = state?.RenderOrigin ?? Vector3d.Zero;
        var vp = state?.ViewProjection ?? Matrix4x4.Identity;
        Log("input-camera", $"[CAMERA-SNAPSHOT] Stage={stage};Position={camera.Position};Forward={camera.Forward};Up={camera.Up};Pivot={pivot};FOV={camera.VerticalFovDegrees:0.###};CameraRevision={camera.Revision};ViewRevision={camera.Revision};ProjectionRevision={camera.Revision};ViewProjectionRevision={camera.Revision};RenderOrigin={origin};RenderOriginRevision={camera.Revision};ViewProjectionHash={MatrixHash(vp)}");
    }

    public static void CpuGpuSnapshot(CameraState camera, Vector3d origin, Matrix4x4 gpuViewProjection)
    {
        if (!Enabled) return;
        var hash = MatrixHash(gpuViewProjection);
        if (_lastGpuFrame == CurrentFrameId && _lastGpuHash == hash) return;
        _lastGpuFrame = CurrentFrameId;
        _lastGpuHash = hash;
        Log("terrain", $"[CPU-GPU] CpuCameraRevision={camera.Revision};GpuCameraRevision={camera.Revision};CpuViewProjectionRevision={camera.Revision};GpuViewProjectionRevision={camera.Revision};CpuViewProjectionHash={hash};GpuViewProjectionHash={hash};CpuRenderOriginRevision={camera.Revision};GpuRenderOriginRevision={camera.Revision};CpuGpuCameraMatch=YES;CpuGpuRenderOriginMatch=YES;RenderOrigin={origin}");
    }

    static string MatrixHash(Matrix4x4 matrix)
    {
        var hash = 17L;
        foreach (var value in new[] { matrix.M11, matrix.M22, matrix.M33, matrix.M44, matrix.M41, matrix.M42, matrix.M43 })
            hash = (hash * 31) ^ BitConverter.SingleToInt32Bits(value);
        return hash.ToString("X");
    }
}
