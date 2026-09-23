using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticR1PopupRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticR1PopupRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Generic_popup_root_is_registered_without_diagnostic_popup_host()
    {
        _fixture.Run(() =>
        {
            var popup = new Popup { Child = new Button { Content = "Popup" } };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 180, Content = new Grid { Children = { host, popup } } };
            window.Show(); window.UpdateLayout(); popup.IsOpen = true; Dispatcher.UIThread.RunJobs();
            var root = TopLevel.GetTopLevel(popup.Child);
            if (root is not null)
            {
                var field = typeof(DiagnosticOverlayHost).GetField("_probeRoots", BindingFlags.Instance | BindingFlags.NonPublic);
                var roots = (IEnumerable<TopLevel>)field!.GetValue(host)!; Assert.Contains(root, roots);
            }
            popup.IsOpen = false; window.Close();
        });
    }
}
