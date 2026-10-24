using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Observability.Probe.Frames;

public sealed class ViewportProbeFrameTests
{
    [Fact]
    public void DoesNotClaimUnmeasuredCpuGpuAgreement()
    {
        var line = ViewportProbe.FormatCpuGpuEvidence(
            12, new Vector3d(1, 2, 3), "ABC123");
        Assert.Contains("CpuCameraRevision=12", line);
        Assert.Contains("GpuMatrixArgumentHash=ABC123", line);
        Assert.Contains("GpuCameraRevision=UNKNOWN", line);
        Assert.Contains("CpuViewProjectionHash=UNKNOWN", line);
        Assert.Contains("GpuViewProjectionHash=UNKNOWN", line);
        Assert.Contains("CpuGpuCameraMatch=UNKNOWN", line);
        Assert.Contains("CpuGpuRenderOriginMatch=UNKNOWN", line);
        Assert.DoesNotContain("CpuGpuCameraMatch=YES", line);
    }
}
