using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticMappedDisplayTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticMappedDisplayTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Mapped_component_card_still_titles_the_tracked_target()
    {
        _fixture.Run(() =>
        {
            var target = new XYButton { Name = "SaveAction", Content = "保存" };
            var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
            var card = new DiagnosticFloatingCard(snapshot, _ => Task.CompletedTask);
            Assert.Equal("SaveAction", card.FindControl<TextBlock>("Title")?.Text);
        });
    }

    [Fact]
    public void Nested_button_content_uses_visible_text_instead_of_panel_type()
    {
        _fixture.Run(() =>
        {
            var target = new XYButton
            {
                Content = new StackPanel { Children = { new TextBlock { Text = "打开" } } }
            };
            var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
            Assert.Equal("打开", snapshot.TargetDisplayName);
            Assert.Equal("打开", snapshot.Text);
        });
    }
}
