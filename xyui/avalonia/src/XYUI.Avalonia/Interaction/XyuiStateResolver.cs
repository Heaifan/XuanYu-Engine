namespace XYUI.Avalonia.Interaction;

public readonly record struct XyuiResolvedState(
    string? BackgroundToken,
    string? BorderToken,
    string? ForegroundToken,
    string? FocusOutlineToken,
    string? SelectionIdentityToken)
{
    public bool HasFocusOutline => FocusOutlineToken is not null;
}

public readonly record struct XyuiSemanticResolution(
    XyuiSemanticStatus Status,
    string? BackgroundToken,
    string? BorderToken,
    string? ForegroundToken);

public static class XyuiStateResolver
{
    public static XyuiResolvedState Resolve(XyuiStateSnapshot state) => new(
        Background(state), Border(state), Foreground(state),
        state.FocusVisible && !state.Has(XyuiInteractionFacts.Disabled)
            ? "XY.Border.Color.Focus" : null,
        state.Has(XyuiInteractionFacts.Selected) ? "XY.Border.Color.Selected" : null);

    public static XyuiSemanticResolution ResolveSemantic(XyuiSemanticStatus status) => status switch
    {
        XyuiSemanticStatus.Success => Semantic(status, "Success"),
        XyuiSemanticStatus.Info => Semantic(status, "Info"),
        XyuiSemanticStatus.Warning => Semantic(status, "Warning"),
        XyuiSemanticStatus.Error => Semantic(status, "Error"),
        _ => new(XyuiSemanticStatus.None, null, null, null),
    };

    static string? Background(XyuiStateSnapshot state)
    {
        if (state.Has(XyuiInteractionFacts.Disabled)) return XyuiInteractionState.DisabledBackgroundToken;
        if (state.Has(XyuiInteractionFacts.Pressed)) return XyuiInteractionState.PressedToken;
        if (state.Has(XyuiInteractionFacts.Hover)) return XyuiInteractionState.HoverToken;
        if (state.Has(XyuiInteractionFacts.DropTarget)) return XyuiInteractionState.DropTargetBackgroundToken;
        if (state.Has(XyuiInteractionFacts.Dragging)) return XyuiInteractionState.DraggingToken;
        if (state.Has(XyuiInteractionFacts.Locked)) return XyuiInteractionState.LockedBackgroundToken;
        if (state.Has(XyuiInteractionFacts.ReadOnly)) return XyuiInteractionState.ReadOnlyBackgroundToken;
        if (state.Has(XyuiInteractionFacts.Active)) return XyuiInteractionState.ActiveToken;
        return state.Has(XyuiInteractionFacts.Selected) ? XyuiInteractionState.SelectedToken : null;
    }

    static string? Border(XyuiStateSnapshot state)
    {
        if (state.Has(XyuiInteractionFacts.Disabled)) return XyuiInteractionState.DisabledBorderToken;
        if (state.Has(XyuiInteractionFacts.DropTarget)) return XyuiInteractionState.DropTargetBorderToken;
        if (state.Has(XyuiInteractionFacts.Locked)) return XyuiInteractionState.LockedBorderToken;
        if (state.Has(XyuiInteractionFacts.ReadOnly)) return XyuiInteractionState.ReadOnlyBorderToken;
        return state.Has(XyuiInteractionFacts.Selected) ? "XY.Border.Color.Selected" : null;
    }

    static string? Foreground(XyuiStateSnapshot state)
    {
        if (state.Has(XyuiInteractionFacts.Disabled)) return XyuiInteractionState.DisabledTextToken;
        if (state.Has(XyuiInteractionFacts.Locked)) return XyuiInteractionState.LockedTextToken;
        return state.Has(XyuiInteractionFacts.ReadOnly) ? XyuiInteractionState.ReadOnlyTextToken : null;
    }

    static XyuiSemanticResolution Semantic(XyuiSemanticStatus status, string name) => new(
        status, $"XY.Semantic.{name}.Background", $"XY.Semantic.{name}.Border", $"XY.Semantic.{name}.Text");
}
