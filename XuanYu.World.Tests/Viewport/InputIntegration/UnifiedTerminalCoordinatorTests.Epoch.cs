using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed partial class UnifiedTerminalCoordinatorTests
{
    [Fact]
    public void New_direct_interaction_epoch_reopens_terminal_handling_once()
    {
        var notifications = new List<EditorPointerEventKind>();
        var composition = new ViewportInputComposition(
            [new ConsumerProbe()], new CaptureProbe(), notifications.Add);
        composition.Terminate(EditorPointerEventKind.Escape);
        composition.Terminate(EditorPointerEventKind.Escape);
        composition.BeginInteractionEpoch();
        composition.Terminate(EditorPointerEventKind.Escape);
        Assert.Equal(2, notifications.Count);
    }
}
