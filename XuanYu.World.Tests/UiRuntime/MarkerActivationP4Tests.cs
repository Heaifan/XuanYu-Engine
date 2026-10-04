using System.Reflection;
using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;
public sealed class MarkerActivationP4Tests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-p4-{Guid.NewGuid():N}");

    [Fact]
    public async Task Valid_map_context_activates_real_point_session()
    {
        var vm = await CreateEditableMarkerVmAsync();
        Assert.True(await vm.BeginMarkerPlacementAsync());
        Assert.True(vm.IsMarkerPlacementTool);
        Assert.True(vm.IsUnifiedMarkerDrawingActive);
        Assert.Equal(DrawingPrimitiveKind.Point, vm.UnifiedDrawingSession!.PrimitiveKind);
    }

    [Fact]
    public async Task Missing_map_id_fails_activation_before_session_input_or_world_commit()
    {
        var vm = await CreateEditableMarkerVmAsync();
        SetManifestId(vm, null);
        var activated = await vm.BeginMarkerPlacementAsync();
        var dispatch = vm.ViewportInput.Router.Dispatch(Event(EditorPointerEventKind.Pressed));

        Assert.False(activated);
        Assert.Null(vm.UnifiedDrawingSession);
        Assert.Empty(vm.MapSession.CurrentMap.Markers);
        Assert.NotEqual(ViewportInputDispatchKind.Ignored, dispatch.Kind);
    }

    [Fact]
    public async Task Marker_activation_failure_reports_marker_domain_not_region_domain()
    {
        var vm = await CreateEditableMarkerVmAsync();
        SetManifestId(vm, null);
        Assert.False(await vm.BeginMarkerPlacementAsync());
        Assert.Contains("标记", vm.FooterMessage);
        Assert.Contains("点", vm.FooterMessage);
        Assert.DoesNotContain("区域绘制已阻止", vm.FooterMessage);
    }

    [Fact]
    public async Task New_and_opened_editable_map_exposes_legal_current_map_id_to_activation()
    {
        var vm = await CreateEditableMarkerVmAsync();
        Assert.True(vm.MapSession.CurrentMap.MapId.IsValid);
        Assert.Equal(vm.MapSession.CurrentMap.MapId.Value, vm.CurrentMapManifest.Id);
        var path = Path.Combine(_root, "map-open.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        Assert.True(await vm.OpenMapManifestAsync(path));
        Assert.Equal(vm.MapSession.CurrentMap.MapId.Value, vm.CurrentMapManifest.Id);
        Assert.True(await vm.BeginMarkerPlacementAsync());
    }

    [Fact]
    public async Task Activation_failure_is_independent_of_router_dispatch_result()
    {
        var vm = await CreateEditableMarkerVmAsync();
        SetManifestId(vm, null);
        var activationFailed = !await vm.BeginMarkerPlacementAsync();
        vm.ViewportInput.Router.Dispatch(Event(EditorPointerEventKind.Pressed));
        Assert.True(activationFailed);
        Assert.Null(vm.UnifiedDrawingSession);
        Assert.Empty(vm.MapSession.CurrentMap.Markers);
    }

    async Task<UiVm> CreateEditableMarkerVmAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Marker;
        Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("地图标记");
        return vm;
    }

    static void SetManifestId(UiVm vm, string? id)
    {
        var owner = (MapManifestOwner)typeof(UiVm).GetField("_mapManifestOwner",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        owner.Modify(vm.CurrentMapManifest with { Id = id! });
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind) => new(kind, new(400, 300),
        EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 1, new("p4"), 1);

    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}
