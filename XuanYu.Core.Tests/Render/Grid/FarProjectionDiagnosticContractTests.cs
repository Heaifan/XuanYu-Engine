using System.IO;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class FarProjectionDiagnosticContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    [Fact]
    public void Diagnostic_captures_shared_projection_boundary_without_mutating_it()
    {
        var source = File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Render", "Grid",
            "VulkanClearFrameOwner.FarDiagnostic.cs"));
        Assert.Contains("WorldRayFactory.FromViewportPoint", source);
        Assert.Contains("t/far", source);
        Assert.Contains("gridLimit=far; axisLimit=far*0.75", source);
        Assert.Contains("if (camera.Revision == _lastFarDiagnosticRevision) return;", source);
        Assert.DoesNotContain("Log(", source);
        Assert.DoesNotContain("FarPlane =", source);
        Assert.DoesNotContain("_referenceGridFrameState =", source);
    }

    [Fact]
    public void Observation_target_flows_only_as_diagnostic_payload()
    {
        var source = File.ReadAllText(Path.Combine(Root, "XuanYu.Editor.UI", "Vm", "Scene",
            "UiVm.RenderProjection.cs"));
        Assert.Contains("_observationCenter", source);
    }
}
