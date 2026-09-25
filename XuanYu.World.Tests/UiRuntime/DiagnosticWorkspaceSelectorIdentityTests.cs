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

    [Fact]
    public void New_named_xyui_button_gets_generated_diagnostic_id_without_manual_binding()
    {
        _fixture.Run(() =>
        {
            var root = new Grid();
            XYDiagnostic.SetDebugId(root, "XYE.TEST.ROOT");
            var button = new XYButton { Name = "NewActionButton", Content = "新按钮" };
            root.Children.Add(button);
            var window = new Window { Width = 300, Height = 120, Content = root };
            window.Show(); window.UpdateLayout();
            DiagnosticAutoBinder.Bind(root);
            Assert.Equal("XYE.TEST.ROOT.NEW_ACTION_BUTTON", XYDiagnostic.GetDebugId(button));
            window.Close();
        });
    }
}
