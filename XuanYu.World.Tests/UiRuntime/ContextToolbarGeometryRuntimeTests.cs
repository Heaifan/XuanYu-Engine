using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarGeometryRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarGeometryRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Root_popup_geometry_does_not_include_submenu_width()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            var window = host.Show(toolbar, 1000, 180);
            var popup = toolbar.FindControl<Popup>("DrawMenuPopup")!;
            var root = toolbar.FindControl<XYMenu>("DrawMenu")!;
            toolbar.FindControl<XYSplitButton>("DrawSplitButton")!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            var rootWidth = root.Bounds.Width;
            var rootX = root.TranslatePoint(default, window)!.Value.X;
            Assert.IsType<DiagnosticPopupHost>(popup.Child);
            root.Items.OfType<XYMenuItem>().First().Activate();
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            Assert.Equal(rootWidth, root.Bounds.Width, 1);
            Assert.Equal(rootX, root.TranslatePoint(default, window)!.Value.X, 1);
            Assert.True(toolbar.FindControl<Popup>("DrawSubMenuPopup")!.IsOpen);
            Assert.True(toolbar.FindControl<XYMenu>("DrawChildMenu")!.DesiredSize.Width < 260);
        });
    }

    [Fact]
    public void Popup_diagnostic_bounds_follow_open_close_and_reopen()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            vm.RunCommand.Execute("诊断模式");
            vm.RunCommand.Execute("区域边界");
            var toolbar = new ContextToolBar { DataContext = vm }; host.Show(toolbar, 1000, 180);
            var rootHost = toolbar.FindControl<DiagnosticPopupHost>("DrawMenuPopupHost")!;
            var childHost = toolbar.FindControl<DiagnosticPopupHost>("DrawSubMenuPopupHost")!;
            var rootPopup = toolbar.FindControl<Popup>("DrawMenuPopup")!;
            toolbar.FindControl<XYSplitButton>("DrawSplitButton")!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            Assert.True(rootPopup.IsOpen && rootHost.ActiveRectangleCount == 1 && rootHost.ActiveLabelCount == 1,
                $"open={rootPopup.IsOpen}, rect={rootHost.ActiveRectangleCount}, label={rootHost.ActiveLabelCount}, data={rootHost.DataContext?.GetType().Name ?? "null"}");
            rootPopup.IsOpen = false; Dispatcher.UIThread.RunJobs();
            Assert.False(rootHost.IsDiagnosticVisible);
            toolbar.FindControl<XYSplitButton>("DrawSplitButton")!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            toolbar.FindControl<XYMenu>("DrawMenu")!.Items.OfType<XYMenuItem>().First().Activate();
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            Assert.True(childHost.ActiveRectangleCount == 1 && childHost.ActiveLabelCount == 1);
            toolbar.FindControl<Popup>("DrawSubMenuPopup")!.IsOpen = false; Dispatcher.UIThread.RunJobs();
            Assert.False(childHost.IsDiagnosticVisible);
        });
    }
}
