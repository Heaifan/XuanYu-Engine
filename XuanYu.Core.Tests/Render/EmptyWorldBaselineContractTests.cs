using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Editor.Composition;
using XuanYu.World;
using XuanYu.World.Scene;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class EmptyWorldBaselineContractTests
{
    static readonly ViewportState Viewport = new(0, 0, 1920, 1080, 1920, 1080, 1, 1);

    [Fact]
    public void EmptyWorld_DrawPlan_DoesNotContainReferencePlaneSurface()
    {
        var projection = EmptyProjection();

        Assert.False(projection.HasMap);
        Assert.False(projection.HasTerrain);
        Assert.True(projection.HasReferencePlane);
        var kinds = RenderDrawPlan.GetFrameDrawPlan(projection).Select(e => e.Kind).ToArray();
        Assert.Equal(new[]
        {
            RenderDrawKind.EditorBackground,
            RenderDrawKind.EditorReferenceGrid,
            RenderDrawKind.WorldOrigin,
            RenderDrawKind.NavigationGizmo
        }, kinds);
    }

    [Fact]
    public void Empty_world_first_frame_is_ready_without_input()
    {
        var vm = CreateEmptyWorldVm();
        vm.UpdateViewportFrame(1920, 1080);
        var result = vm.RenderProjection;

        Assert.True(result.Success);
        Assert.True(result.Projection.HasReferencePlane);
        Assert.True(result.Projection.AssistState.ShowGrid);
        Assert.True(IsFinite(result.Projection.Camera.ToViewProjection(Viewport)));
    }

    [Fact]
    public void New_scene_returns_to_empty_world_without_map_roundtrip()
    {
        var vm = CreateEmptyWorldVm();

        vm.NewBlankScene();

        Assert.False(vm.RenderProjection.Projection.HasMap);
        Assert.True(vm.RenderProjection.Projection.HasReferencePlane);
        Assert.Empty(vm.RenderSnapshot.Entities);
    }

    static RenderProjection EmptyProjection() => new(
        new RenderCameraProjection(new(0, -10, 10), new(0, 0, -1), Vector3d.UnitY,
            60, 0.1, 100000, 1), [], false, default,
        Assist: EditorViewportAssistState.Default,
        Map: MapRenderSnapshot.Empty,
        ReferencePlane: ReferencePlaneRenderSnapshot.Default);

    static UiVm CreateEmptyWorldVm() => new(null,
        EditorRuntimeComposition.CreateScene(new GridWorldPartitionStrategy(5), false),
        EditorRuntimeComposition.CreateEmptyWorldMapSession(() => true),
        () => true);

    static bool IsFinite(ViewProjectionState state) =>
        float.IsFinite(state.ViewProjection.M11) && float.IsFinite(state.ViewProjection.M44);
}
