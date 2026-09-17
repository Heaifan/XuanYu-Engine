using System.IO;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class WorkspaceSelectorR2ContractTests
{
    readonly UiHeadlessFixture _fixture;

    public WorkspaceSelectorR2ContractTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Selector_has_one_workspace_selection_control()
    {
        var source = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml") +
                     Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml.cs");
        Assert.Contains("XYWorkspaceSwitcher", source);
        Assert.DoesNotContain("XYMenuBarItem", source);
        Assert.DoesNotContain("Shortcut=\"暂未开放\"", source);
        Assert.DoesNotContain("ToggleEditorModeCommand", source);
        Assert.DoesNotContain("SwitchWorkspaceCommand", source);
        Assert.DoesNotContain("DoubleTapped=", source);
    }

    [Fact]
    public void Workspace_menu_exposes_current_and_disabled_options()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var states = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var selector = new WorkspaceSelector { DataContext = vm };
            host.Show(selector, 420, 48);
            selector.UpdateLayout(); var switcher = selector.GetVisualDescendants().OfType<XYWorkspaceSwitcher>().Single();
            switcher.Open(); Dispatcher.UIThread.RunJobs();
            var items = switcher.WorkspaceMenu.Items.OfType<XYMenuItem>().ToArray();
            var feature = items.Single(x => x.Label == "要素编辑"); feature.Activate();
            switcher.Open(); Dispatcher.UIThread.RunJobs();
            var final = switcher.WorkspaceMenu.Items.OfType<XYMenuItem>().ToArray();
            return (final.Select(x => x.Label).ToArray(), final.Single(x => x.Label == "要素编辑").IsChecked,
                final.Single(x => x.Label == "场景编辑（暂未开放）").IsEnabled,
                final.Single(x => x.Label == "调试（暂未开放）").IsEnabled);
        });

        Assert.Equal(["要素编辑", "场景编辑（暂未开放）", "调试（暂未开放）", "管理工作区..."], states.Item1);
        Assert.True(states.Item2);
        Assert.False(states.Item3);
        Assert.False(states.Item4);
    }

    [Fact]
    public void Top_declares_one_environment_menu()
    {
        var source = Read("XuanYu.Editor.UI", "Top", "ViewModule.axaml");
        Assert.Equal(1, Count(source, "Label=\"环境\""));
    }

    [Fact]
    public void Area_a_uses_canonical_xyui_menu_apis()
    {
        var workspace = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml");
        var file = Read("XuanYu.Editor.UI", "Top", "FileModule.axaml");
        var view = Read("XuanYu.Editor.UI", "Top", "ViewModule.axaml");
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");

        Assert.Contains("XYWorkspaceSwitcher", workspace + Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml.cs"));
        Assert.DoesNotContain("XYMenuBarItem", workspace);
        Assert.DoesNotContain("点要素编辑", workspace);
        Assert.DoesNotContain("OpenPointFeatureEditorCommand", workspace);
        Assert.DoesNotContain("ToggleType=", workspace);
        Assert.DoesNotContain("Header=", workspace + file + view);
        Assert.DoesNotContain("<Style Selector=\"Menu\"", top);
        Assert.DoesNotContain("<Style Selector=\"MenuItem\"", top);
    }

    static string Read(params string[] path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(path)));

    static int Count(string text, string value) => text.Split(value).Length - 1;

}
