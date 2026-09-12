using Avalonia.Controls;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2Fix4RegionInspectorRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2Fix4RegionInspectorRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Region_inspector_is_property_only_without_total_scroll_surface()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
            var panel = new InspectorPanel { DataContext = vm }; host.Show(panel, 480, 720); panel.UpdateLayout();
            return (Regions: UiRuntimeTestHost.Descendants<RegionalAuthoringPanel>(panel).Count(),
                HasInspectorScroll: panel.FindControl<ScrollViewer>("InspectorScrollViewer") is not null);
        });
        Assert.Equal(0, state.Regions); Assert.False(state.HasInspectorScroll);
    }
}
