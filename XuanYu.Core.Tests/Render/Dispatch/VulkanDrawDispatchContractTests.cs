using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Core.Tests.Render.Dispatch;

public sealed class VulkanDrawDispatchContractTests
{
    public static TheoryData<RenderDrawKind, string> Owners => new()
    {
        { RenderDrawKind.EditorBackground, "EditorBackground" },
        { RenderDrawKind.EditorReferenceGrid, "EditorReferenceGrid" },
        { RenderDrawKind.WorldOrigin, "WorldOrigin" },
        { RenderDrawKind.WorldAxes, "WorldAxes" },
        { RenderDrawKind.MapGround, "MapGround" },
        { RenderDrawKind.MapBounds, "MapBounds" },
        { RenderDrawKind.Terrain, "Terrain" },
        { RenderDrawKind.MapVectorOverlay, "VectorOverlay" },
        { RenderDrawKind.EntityFill, "Entity" },
        { RenderDrawKind.EntityOutline, "Entity" },
        { RenderDrawKind.MoveGizmo, "Gizmo" },
        { RenderDrawKind.RotateGizmo, "Gizmo" },
        { RenderDrawKind.ScaleGizmo, "Gizmo" },
        { RenderDrawKind.ScaleIndicatorOverlay, "ScaleIndicatorOverlay" },
        { RenderDrawKind.NavigationGizmo, "NavigationGizmo" },
        { RenderDrawKind.EditorViewPlaneGrid, "EditorViewPlaneGrid" }
    };

    [Theory]
    [MemberData(nameof(Owners))]
    public void Each_kind_resolves_to_one_logical_owner(RenderDrawKind kind,
        string expectedOwner)
    {
        Assert.Equal(expectedOwner, VulkanClearFrameOwner.ResolveDrawOwner(kind).ToString());
    }

    [Fact]
    public void Unknown_kind_fails_explicitly()
    {
        var unknown = (RenderDrawKind)int.MaxValue;

        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => VulkanClearFrameOwner.ResolveDrawOwner(unknown));

        Assert.Contains("Unsupported RenderDrawKind", error.Message);
    }

    [Fact]
    public void Every_declared_kind_has_one_contract_entry()
    {
        Assert.All(Enum.GetValues<RenderDrawKind>(), kind =>
            Assert.IsType<VulkanClearFrameOwner.DrawOwner>(
                VulkanClearFrameOwner.ResolveDrawOwner(kind)));
    }

    [Fact]
    public void Terrain_readiness_is_independent_of_main_pipeline()
    {
        var mainReadyTerrainMissing = new VulkanClearFrameOwner.PipelineReadiness(
            Main: true, Sky: false, Grid: false, Origin: false, Axes: false,
            Navigation: false, ScaleIndicator: false, ViewPlaneGrid: false,
            Terrain: false, VectorOverlay: false);
        var mainMissingTerrainReady = mainReadyTerrainMissing with { Main = false, Terrain = true };

        Assert.False(VulkanClearFrameOwner.IsPipelineReady(
            VulkanClearFrameOwner.DrawOwner.Terrain, mainReadyTerrainMissing));
        Assert.True(VulkanClearFrameOwner.IsPipelineReady(
            VulkanClearFrameOwner.DrawOwner.Terrain, mainMissingTerrainReady));
    }

    [Fact]
    public void Vector_overlay_requires_its_dedicated_pipeline_contract()
    {
        var mainOnly = new VulkanClearFrameOwner.PipelineReadiness(
            Main: true, Sky: false, Grid: false, Origin: false, Axes: false,
            Navigation: false, ScaleIndicator: false, ViewPlaneGrid: false,
            Terrain: false, VectorOverlay: false);
        var vectorReady = mainOnly with { VectorOverlay = true };

        Assert.False(VulkanClearFrameOwner.IsPipelineReady(
            VulkanClearFrameOwner.DrawOwner.VectorOverlay, mainOnly));
        Assert.True(VulkanClearFrameOwner.IsPipelineReady(
            VulkanClearFrameOwner.DrawOwner.VectorOverlay, vectorReady));
    }
}
