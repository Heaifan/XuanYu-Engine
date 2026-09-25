using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticAutoIdRegressionTests : IDisposable
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticAutoIdRegressionTests(UiHeadlessFixture fixture) => _fixture = fixture;
    public void Dispose() => DiagnosticRegistry.Clear();

    [Fact]
    public void Dynamic_xyicon_buttons_keep_unique_ids_after_registry_refresh()
    {
        _fixture.Run(() =>
        {
            var root = new Grid();
            var first = new XYIconButton(); root.Children.Add(first);
            var window = new Window { Width = 240, Height = 120, Content = root };
            window.Show(); window.UpdateLayout(); Refresh(root);
            var firstId = XYDiagnostic.GetDebugId(first);
            Assert.Equal("XYE.AUTO.XYICON_BUTTON", firstId);

            DiagnosticRegistry.Clear();
            var second = new XYIconButton(); root.Children.Add(second);
            Refresh(root);

            var ids = new[] { first, second }.Select(XYDiagnostic.GetDebugId).ToArray();
            Assert.All(ids, id => Assert.NotNull(id));
            Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
            Assert.All(ids, id => Assert.Same(
                id is null ? null : id == XYDiagnostic.GetDebugId(first) ? first : second,
                DiagnosticRegistry.Targets[id!]
            ));
            window.Close();
        });
    }

    [Fact]
    public void Rebuild_repairs_duplicate_auto_ids_but_rejects_manual_duplicates()
    {
        var first = new Border(); var second = new Border();
        XYDiagnostic.SetDebugId(first, "XYE.AUTO.XYICON_BUTTON");
        XYDiagnostic.SetDebugId(second, "XYE.AUTO.XYICON_BUTTON");
        DiagnosticRegistry.Rebuild(new[] { first, second });
        Assert.NotEqual(XYDiagnostic.GetDebugId(first), XYDiagnostic.GetDebugId(second));
        Assert.Equal(2, DiagnosticRegistry.Targets.Count);

        DiagnosticRegistry.Clear();
        var third = new Border(); var fourth = new Border();
        XYDiagnostic.SetDebugId(third, "XYE.AUTO.XYICON_BUTTON");
        XYDiagnostic.SetDebugId(fourth, "XYE.AUTO.XYICON_BUTTON");
        var error = Record.Exception(() =>
        {
            DiagnosticRegistry.Register(third); DiagnosticRegistry.Register(fourth);
        });
        Assert.Null(error); Assert.Equal(2, DiagnosticRegistry.Targets.Count);

        var manual = new Border(); XYDiagnostic.SetDebugId(manual, "XYE.TEST.MANUAL");
        Assert.Throws<InvalidOperationException>(() =>
            DiagnosticRegistry.Rebuild(new[] { first, manual, new BorderWithId("XYE.TEST.MANUAL") }));
    }

    static void Refresh(Control root)
    {
        DiagnosticAutoBinder.Bind(root);
        DiagnosticRegistry.Rebuild(root.GetVisualDescendants().OfType<Control>().Prepend(root));
    }

    sealed class BorderWithId : Border
    {
        public BorderWithId(string id) => XYDiagnostic.SetDebugId(this, id);
    }
}
