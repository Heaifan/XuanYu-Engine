using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3TableOfContentsTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    static XYTocSection[] Sections => [new("intro", "概览", 1), new("setup", "配置", 1), new("map", "地图基础", 2, "setup"), new("data", "数据集", 2, "setup"), new("api", "API", 1)];
    public XYUI3TableOfContentsTests(XyuiHeadlessFixture fx) => _fx = fx;
    [Fact] public void Desktop_has_header_without_tree_icons_and_level2_guide() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var toc = new XYTableOfContents(new XYTableOfContentsState(Sections, "data")); var window = XyuiBatchTestHost.Show(toc); Dispatcher.UIThread.RunJobs(); Assert.Contains(toc.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "本页目录"); Assert.DoesNotContain(toc.GetVisualDescendants().OfType<XYIcon>(), _ => true); Assert.Contains(toc.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-toc-level-guide")); window.Close(); });
    [Fact] public void Current_child_marks_parent_active_and_has_left_accent() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var toc = new XYTableOfContents(new XYTableOfContentsState(Sections, "data")); var window = XyuiBatchTestHost.Show(toc); Dispatcher.UIThread.RunJobs(); var current = toc.GetVisualDescendants().OfType<XYButton>().Single(x => x.Classes.Contains("xyui-toc-current")); Assert.Contains(toc.GetVisualDescendants().OfType<XYButton>(), x => x.Classes.Contains("xyui-toc-parent-active")); Assert.Contains(current.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-toc-current-accent") && x.IsVisible); window.Close(); });
    [Fact] public void Compact_trigger_has_path_chevron_and_popup_width() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var toc = new XYTableOfContents(new XYTableOfContentsState(Sections, "data"), XYTableOfContentsVariant.Compact); var window = XyuiBatchTestHost.Show(toc); Dispatcher.UIThread.RunJobs(); var trigger = toc.GetVisualDescendants().OfType<XYButton>().Single(); Assert.Contains(trigger.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "本页目录"); Assert.Contains(trigger.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "配置 / 数据集"); Assert.Contains(trigger.GetVisualDescendants().OfType<XYIcon>(), x => x.Icon == XYUI.Avalonia.Vector.XyuiVectorIcon.ChevronDown); toc.SelectSection("data"); trigger.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent)); Assert.Equal(trigger.Bounds.Width, toc.Popup.Width, 1); window.Close(); });
    [Fact] public void Request_requires_accept_reject_is_terminal_and_current_is_not_repeated() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var toc = new XYTableOfContents(new XYTableOfContentsState(Sections, "intro")); var count = 0; toc.SectionRequested += (_, request) => { count++; request.Reject(); request.Accept(); }; toc.SelectSection("data"); toc.SelectSection("intro"); Assert.Equal("intro", toc.CurrentSectionId); Assert.Equal(1, count); });
    [Fact] public void Commit_returns_false_for_current_and_state_is_shared() { var state = new XYTableOfContentsState(Sections, "intro"); var changed = 0; state.Changed += (_, _) => changed++; Assert.False(state.Commit("intro")); Assert.Equal(0, changed); var compact = new XYTableOfContents(state, XYTableOfContentsVariant.Compact); Assert.Same(state, compact.State); }
    [Fact] public void Level3_is_not_rendered() { var toc = new XYTableOfContents([new XYTocSection("a", "A", 1), new XYTocSection("deep", "Deep", 3)]); Assert.DoesNotContain(toc.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "Deep"); }
}
