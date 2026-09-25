using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticFloatingCardXyuiTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticFloatingCardXyuiTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Floating_card_uses_xyui_text_status_and_buttons()
    {
        _fixture.Run(() =>
        {
            var target = new XYMenuItem { Label = "点标记" };
            var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
            var card = new DiagnosticFloatingCard(snapshot, _ => Task.CompletedTask);
            Assert.IsType<XYButton>(card.FindControl<XYButton>("Pin"));
            Assert.IsType<XYButton>(card.FindControl<XYButton>("CopyAi"));
            Assert.IsType<XYButton>(card.FindControl<XYButton>("Expand"));
            Assert.IsType<XYButton>(card.FindControl<XYButton>("Close"));
            Assert.IsType<XYText>(card.FindControl<XYText>("Title"));
            Assert.IsType<XYText>(card.FindControl<XYText>("Identity"));
            Assert.IsType<XYText>(card.FindControl<XYText>("Details"));
            Assert.IsType<XYStatusBadge>(card.FindControl<XYStatusBadge>("Locked"));
        });
    }

    [Fact]
    public void Xyui_menu_item_has_catalog_identity_and_label()
    {
        _fixture.Run(() =>
        {
            var target = new XYMenuItem { Label = "点标记" };
            var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
            Assert.Equal("点标记", snapshot.TargetDisplayName);
            Assert.Equal("XYUI3", snapshot.Identity.Number);
            Assert.Equal("XYUI-3-02", snapshot.Identity.CatalogId);
        });
    }

    [Fact]
    public void Floating_card_title_uses_dark_xyui_layout_on_light_panel()
    {
        _fixture.Run(() =>
        {
            var snapshot = DiagnosticElementSnapshot.Capture(
                DiagnosticProbeResolver.Resolve(new XYMenuItem { Label = "点标记" }));
            var card = new DiagnosticFloatingCard(snapshot, _ => Task.CompletedTask);
            var title = card.FindControl<XYText>("Title")!;
            Assert.IsType<Grid>(card.FindControl<Grid>("Header"));
            Assert.NotEqual(Colors.White, ((SolidColorBrush)title.Foreground!).Color);
        });
    }
}
