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
}
