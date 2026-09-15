using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticSnapshotTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticSnapshotTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Formatter_matches_literal_and_missing_entity_is_na()
    {
        var snapshot = new DiagnosticSnapshot("XYE.VIEWPORT", "XYE.AREA.CENTER", "v1-rz",
            "Empty", "N/A", true, false, new Rect(12.5, 20, 80.25, 30),
            new Size(320, 200.5), 1.25, "Light");
        var expected = """
            [XYengine Diagnostic]

            DebugId: XYE.VIEWPORT

            AreaId: XYE.AREA.CENTER

            Version: v1-rz

            SelectionType: Empty

            EntityId: N/A

            Visible: True

            Enabled: False

            Bounds: X=12.5 Y=20 W=80.25 H=30

            Window: 320x200.5

            RenderScale: 1.25

            Theme: Light
            """.ReplaceLineEndings(Environment.NewLine);

        Assert.Equal(expected, snapshot.ToString());
    }

    [Fact]
    public void Factory_captures_nearest_area_and_effective_top_level_facts()
    {
        _fixture.Run(() =>
        {
            var target = new Border { Width = 80, Height = 30, Margin = new Thickness(12, 20, 0, 0) };
            XYDiagnostic.SetDebugId(target, "XYE.VIEWPORT");
            var area = new Grid { IsEnabled = false, Children = { target } };
            XYDiagnostic.SetAreaId(area, "XYE.AREA.CENTER");
            var vm = new UiVm(null, seedInitialScene: false);
            var window = new Window { Width = 320, Height = 200, DataContext = vm,
                RequestedThemeVariant = ThemeVariant.Light, Content = area };
            window.Show(); window.UpdateLayout();

            var result = DiagnosticSnapshotFactory.Capture(target);
            var origin = target.TranslatePoint(default, window)!.Value;

            Assert.Equal("XYE.AREA.CENTER", result.AreaId);
            Assert.Equal(target.IsEffectivelyVisible, result.Visible);
            Assert.Equal(target.IsEffectivelyEnabled, result.Enabled);
            Assert.Equal(new Rect(origin, target.Bounds.Size), result.Bounds);
            Assert.Equal(window.ClientSize, result.Window);
            Assert.Equal(window.RenderScaling, result.RenderScale);
            Assert.Equal(window.ActualThemeVariant.ToString(), result.Theme);
            Assert.Equal("Empty", result.SelectionType);
            Assert.Equal("N/A", result.EntityId);
            window.Close();
        });
    }
}
