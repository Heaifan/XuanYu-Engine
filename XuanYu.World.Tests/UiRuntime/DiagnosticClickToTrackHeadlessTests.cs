using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.Input;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticClickToTrackHeadlessTests
{
    readonly UiHeadlessFixture _fixture;

    public DiagnosticClickToTrackHeadlessTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Native_left_down_dismisses_the_xyui_map_context_menu()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式");
            var host = new VulkanNativeHost { DataContext = vm };
            var overlay = new DiagnosticOverlayHost { DataContext = vm };
            var root = new Grid(); root.Children.Add(host); root.Children.Add(overlay);
            var window = new Window { Width = 320, Height = 240, Content = root };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            var menu = new XYContextMenu();
            typeof(VulkanNativeHost).GetField("_mapContextMenu",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(host, menu);
            menu.Open(host);
            typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(host,
                [new NativePointerMessage(NativePointerMessage.LeftDown, 1, 20, 30, (nint)1, 0, 0, 0)]);
            Assert.False(menu.IsOpen);
            window.Close();
        });
    }
}
