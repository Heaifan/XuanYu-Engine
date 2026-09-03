namespace XYUI.Avalonia.Interaction;

[Flags]
public enum XyuiInteractionFacts
{
    None = 0,
    Hover = 1 << 0,
    Pressed = 1 << 1,
    Selected = 1 << 2,
    Active = 1 << 3,
    Disabled = 1 << 4,
    Dragging = 1 << 5,
    DropTarget = 1 << 6,
    ReadOnly = 1 << 7,
    Locked = 1 << 8,
}

public enum XyuiSemanticStatus
{
    None,
    Success,
    Info,
    Warning,
    Error,
}

public readonly record struct XyuiStateSnapshot(
    XyuiInteractionFacts Interaction,
    bool FocusVisible = false,
    XyuiSemanticStatus Semantic = XyuiSemanticStatus.None)
{
    public bool Has(XyuiInteractionFacts fact) => (Interaction & fact) == fact;
    public static XyuiStateSnapshot Rest => new(XyuiInteractionFacts.None);
}
