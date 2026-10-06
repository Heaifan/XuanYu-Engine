using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3WorkspaceSwitcherProjectionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fixture;
    public XYUI3WorkspaceSwitcherProjectionTests(XyuiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Bound_selection_projects_without_committing_control_state()
    {
        _fixture.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var items = new[] { new XYWorkspaceItem("management", "管理模式"),
                new XYWorkspaceItem("feature", "要素编辑"), new XYWorkspaceItem("future", "场景编辑", false) };
            var switcher = new XYWorkspaceSwitcher(items) { SelectedWorkspaceId = "management" };
            var requests = 0; switcher.WorkspaceChangeRequested += (_, request) => { requests++; request.Accept(); };

            switcher.SelectWorkspace("feature");
            Assert.Equal("管理模式", switcher.CurrentWorkspace);
            Assert.Equal("management", switcher.SelectedWorkspaceId);
            switcher.SelectedWorkspaceId = "feature";
            Assert.Equal("要素编辑", switcher.CurrentWorkspace);
            switcher.SelectWorkspace("future");
            Assert.Equal(1, requests);
        });
    }

    [Fact]
    public void Same_workspace_selection_does_not_raise_duplicate_events()
    {
        _fixture.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("a"),
                new XYWorkspaceItem("a", "A"), new XYWorkspaceItem("b", "B"));
            var changed = 0; switcher.WorkspaceChanged += (_, _) => changed++;

            switcher.SelectWorkspace("a"); switcher.SelectWorkspace("b"); switcher.SelectWorkspace("b");

            Assert.Equal(1, changed);
            Assert.Equal("b", switcher.State.CurrentWorkspaceId);
        });
    }
}
