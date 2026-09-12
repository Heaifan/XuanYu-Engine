using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class RightTabsVisibilityRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public RightTabsVisibilityRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Right_tabs_keep_inspector_hierarchy_and_debug_mutually_exclusive()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var states = host.Run(() =>
        {
            var tabs = new EditorRightTabs { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(tabs, 300, 420); tabs.UpdateLayout();
            var nav = UiRuntimeTestHost.Descendants<XYTabs>(tabs).Single();
            return new[] { "inspector", "hierarchy", "debug", "inspector" }.Select(id =>
            {
                nav.Select(id); Dispatcher.UIThread.RunJobs(); tabs.UpdateLayout();
                return (Inspector: Visible<InspectorPanel>(tabs), Hierarchy: Visible<HierarchyWorkspace>(tabs), Debug: Visible<Grid>(tabs, "DebugWorkspace"));
            }).ToArray();
        });
        Assert.Equal((true, false, false), states[0]); Assert.Equal((false, true, false), states[1]);
        Assert.Equal((false, false, true), states[2]); Assert.Equal((true, false, false), states[3]);
    }

    static bool Visible<T>(Control root) where T : Control => UiRuntimeTestHost.Descendants<T>(root).Single().IsEffectivelyVisible;
    static bool Visible<T>(Control root, string name) where T : Control => UiRuntimeTestHost.Descendants<T>(root).Single(x => x.Name == name).IsEffectivelyVisible;
}
