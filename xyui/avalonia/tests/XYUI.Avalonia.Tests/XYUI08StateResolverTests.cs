using Avalonia.Controls;
using Avalonia.Styling;
using XYUI.Avalonia.Interaction;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI08StateResolverTests
{
    [Fact]
    public void Semantic_facts_can_coexist_and_resolve_once_per_channel()
    {
        var state = new XyuiStateSnapshot(
            XyuiInteractionFacts.Selected | XyuiInteractionFacts.Hover |
            XyuiInteractionFacts.Dragging, true, XyuiSemanticStatus.Info);
        var result = XyuiStateResolver.Resolve(state);
        Assert.Equal("XY.State.Color.Hover", result.BackgroundToken);
        Assert.Equal("XY.Border.Color.Selected", result.BorderToken);
        Assert.Equal("XY.Border.Color.Focus", result.FocusOutlineToken);
        Assert.Equal("XY.Border.Color.Selected", result.SelectionIdentityToken);
        Assert.NotEqual(result.BackgroundToken, result.SelectionIdentityToken);
    }

    [Fact]
    public void Disabled_wins_and_pressed_is_above_hover()
    {
        var disabled = new XyuiStateSnapshot(
            XyuiInteractionFacts.Disabled | XyuiInteractionFacts.Pressed |
            XyuiInteractionFacts.Hover, true);
        Assert.Equal("XY.State.Disabled.Background", XyuiStateResolver.Resolve(disabled).BackgroundToken);
        var pressed = new XyuiStateSnapshot(XyuiInteractionFacts.Pressed | XyuiInteractionFacts.Hover);
        Assert.Equal("XY.State.Color.Pressed", XyuiStateResolver.Resolve(pressed).BackgroundToken);
    }

    [Fact]
    public void Active_is_not_pressed_and_focus_is_independent()
    {
        var result = XyuiStateResolver.Resolve(new(XyuiInteractionFacts.Active, true));
        Assert.Equal("XY.State.Color.Active", result.BackgroundToken);
        Assert.Null(result.BorderToken);
        Assert.True(result.HasFocusOutline);
    }

    [Fact]
    public void Readonly_locked_and_semantic_statuses_remain_distinct()
    {
        var readOnly = XyuiStateResolver.Resolve(new(XyuiInteractionFacts.ReadOnly));
        var locked = XyuiStateResolver.Resolve(new(XyuiInteractionFacts.Locked));
        Assert.NotEqual(readOnly.BackgroundToken, locked.BackgroundToken);
        Assert.NotEqual(readOnly.ForegroundToken, locked.ForegroundToken);
        var warning = XyuiStateResolver.ResolveSemantic(XyuiSemanticStatus.Warning);
        Assert.Equal("XY.Semantic.Warning.Background", warning.BackgroundToken);
        Assert.NotEqual(warning.BackgroundToken, locked.BackgroundToken);
    }

    [Fact]
    public void Drop_target_uses_its_canonical_background_and_border()
    {
        var result = XyuiStateResolver.Resolve(new(XyuiInteractionFacts.DropTarget));
        Assert.Equal("XY.State.Color.DropTarget.Background", result.BackgroundToken);
        Assert.Equal("XY.State.Color.DropTarget.Border", result.BorderToken);
    }

    [Fact]
    public void Focus_does_not_change_the_resolved_geometry_contract()
    {
        var rest = XyuiStateResolver.Resolve(XyuiStateSnapshot.Rest);
        var focused = XyuiStateResolver.Resolve(new(XyuiInteractionFacts.Selected, true));
        Assert.False(rest.HasFocusOutline);
        Assert.Equal("XY.Border.Color.Selected", focused.SelectionIdentityToken);
    }

    [Fact]
    public void Foundation_state_styles_do_not_set_control_size()
    {
        var setters = XyuiInteractionStyles.Create().OfType<Style>()
            .SelectMany(style => style.Setters.OfType<Setter>());
        Assert.DoesNotContain(setters, setter => setter.Property == Control.WidthProperty ||
            setter.Property == Control.HeightProperty);
    }
}
