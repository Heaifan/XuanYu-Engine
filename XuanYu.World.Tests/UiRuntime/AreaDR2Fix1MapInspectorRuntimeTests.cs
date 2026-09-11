using Avalonia.Controls;
using Avalonia.Interactivity;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2Fix1MapInspectorRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2Fix1MapInspectorRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Map_form_uses_number_fields_and_real_xy_buttons()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var form = new MapFormPanel { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(form, 480, 640); form.UpdateLayout();
            return (Numbers: UiRuntimeTestHost.Descendants<XYNumberField>(form).Count(),
                Texts: UiRuntimeTestHost.Descendants<XYTextField>(form).Count(x => x is not XYNumberField),
                Buttons: UiRuntimeTestHost.Descendants<XYButton>(form).Count());
        });
        Assert.Equal(3, result.Numbers); Assert.Equal(0, result.Texts); Assert.Equal(3, result.Buttons);
    }

    [Fact]
    public void Map_id_keeps_only_selectable_text_copy_action()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var page = new MapPagePanel { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(page, 480, 640); page.UpdateLayout();
            var row = page.FindControl<Grid>("MapIdRow");
            return (Row: row, Selectable: row is null ? 0 : UiRuntimeTestHost.Descendants<XYSelectableText>(row).Count(),
                Icons: row is null ? 0 : UiRuntimeTestHost.Descendants<XYIconButton>(row).Count());
        });
        Assert.NotNull(result.Row); Assert.Equal(1, result.Selectable); Assert.Equal(0, result.Icons);
    }

    [Fact]
    public void Map_context_hides_entity_header_owner()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var visible = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var panel = new InspectorPanel { DataContext = vm };
            host.Show(panel, 480, 720); panel.UpdateLayout();
            return panel.FindControl<Grid>("EntityHeader")?.IsEffectivelyVisible;
        });
        Assert.False(visible);
    }

    [Fact]
    public void Number_draft_commits_only_through_apply_and_history_buttons()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var values = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var form = new MapFormPanel { DataContext = vm }; host.Show(form, 480, 640); form.UpdateLayout();
            var width = form.FindControl<XYNumberField>("WidthBoxWide")!; width.Value = 12000; form.UpdateLayout();
            var apply = UiRuntimeTestHost.Descendants<XYButton>(form).Single(x => x.Content?.ToString() == "应用修改");
            var before = vm.MapSession.CurrentMap.SizeMeters.Width; apply.Command!.Execute(apply.CommandParameter);
            var afterApply = vm.MapSession.CurrentMap.SizeMeters.Width;
            var undo = UiRuntimeTestHost.Descendants<XYButton>(form).Single(x => x.Content?.ToString() == "撤销地图修改");
            undo.Command!.Execute(undo.CommandParameter); var afterUndo = vm.MapSession.CurrentMap.SizeMeters.Width;
            var redo = UiRuntimeTestHost.Descendants<XYButton>(form).Single(x => x.Content?.ToString() == "重做地图修改");
            redo.Command!.Execute(redo.CommandParameter);
            return (before, afterApply, afterUndo, AfterRedo: vm.MapSession.CurrentMap.SizeMeters.Width,
                Draft: vm.MapWidthDraft, Field: width.Value);
        });
        Assert.Equal(10000, values.before); Assert.Equal(12000, values.afterApply);
        Assert.Equal(10000, values.afterUndo); Assert.Equal(12000, values.AfterRedo);
    }

    [Fact]
    public void Map_navigation_materializes_paged_navigation()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var height = host.Run(() =>
        {
            var panel = new MapEditorPanel { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(panel, 480, 640); panel.UpdateLayout();
            return panel.FindControl<XYPager>("MapPager")?.Pages.Count ?? 0;
        });
        Assert.Equal(5, height);
    }
}
