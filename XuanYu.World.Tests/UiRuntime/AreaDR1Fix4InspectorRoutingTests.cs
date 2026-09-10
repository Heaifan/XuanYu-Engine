using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR1Fix4InspectorRoutingTests
{
    readonly UiHeadlessFixture _fixture;

    public AreaDR1Fix4InspectorRoutingTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Map_edit_entity_selection_shows_entity_inspector_only()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var visible = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.AddCubeEntity(); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm };
            host.Show(right, 480, 720); right.UpdateLayout();
            return (Map: UiRuntimeTestHost.Descendants<MapFormPanel>(right).Any(x => x.IsEffectivelyVisible),
                Entity: UiRuntimeTestHost.Descendants<EntityInspectorPanel>(right).Count(x => x.IsEffectivelyVisible));
        });

        Assert.False(visible.Map);
        Assert.Equal(1, visible.Entity);
    }

    [Fact]
    public void Map_edit_without_entity_selection_keeps_map_form_route()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var visible = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm };
            host.Show(right, 480, 720); right.UpdateLayout();
            return (Map: UiRuntimeTestHost.Descendants<MapFormPanel>(right).Count(x => x.IsEffectivelyVisible),
                Entity: UiRuntimeTestHost.Descendants<EntityInspectorPanel>(right).Count(x => x.IsEffectivelyVisible));
        });

        Assert.Equal(1, visible.Map);
        Assert.Equal(0, visible.Entity);
    }

    [Fact]
    public void Entity_selection_survives_manage_edit_mode_switch()
    {
        var vm = new UiVm(null, seedInitialScene: false);
        vm.AddCubeEntity();
        Assert.True(vm.IsEntityInspector);

        vm.ToggleEditorMode();
        vm.ToggleEditorMode();

        Assert.True(vm.IsEntityInspector);
    }

    [Fact]
    public void Map_form_uses_xyui_numeric_draft_bindings()
    {
        var source = Read("XuanYu.Editor.UI", "Right", "MapFormPanel.axaml");
        Assert.Equal(3, Count(source, "<xy:XYNumberField"));
        Assert.DoesNotContain("<xy:XYTextField", source);
        foreach (var property in new[] { "MapWidthDraft", "MapDepthDraft", "MapBaseHeightDraft" })
            Assert.Contains(property, source);
    }

    static string Read(params string[] path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(path)));

    static int Count(string source, string value) => source.Split(value).Length - 1;
}
