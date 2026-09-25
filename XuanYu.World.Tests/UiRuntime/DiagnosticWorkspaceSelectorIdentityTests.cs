using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticWorkspaceSelectorIdentityTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticWorkspaceSelectorIdentityTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Workspace_trigger_exposes_complete_diagnostic_identity()
    {
        _fixture.Run(() =>
        {
            var selector = new WorkspaceSelector();
            var window = new Window { Width = 600, Height = 160, Content = selector };
            window.Show(); window.UpdateLayout();
            var switcher = selector.GetVisualDescendants().OfType<XYWorkspaceSwitcher>().Single();
            var result = DiagnosticProbeResolver.Resolve(switcher.Trigger);
            Assert.Equal("WorkspaceSelectorButton", result.Name);
            Assert.Equal("XYE.TOP.WORKSPACE.SELECTOR", result.DebugId);
            Assert.Equal("XYE.TOP.WORKSPACE", result.ParentDebugId);
            Assert.Equal("XYButton", result.ControlType);
            window.Close();
        });
    }
}
