using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYStatusDot : Border
{
    public static readonly StyledProperty<XyuiStatusState> StateProperty =
        AvaloniaProperty.Register<XYStatusDot, XyuiStatusState>(nameof(State), XyuiStatusState.Neutral);

    public XYStatusDot() { Classes.Add("xyui-status-dot"); ApplyState(State); }
    public string CanonicalId => "XYUI-1-11";
    public XyuiStatusState State { get => GetValue(StateProperty); set { SetValue(StateProperty, value); ApplyState(value); } }
    void ApplyState(XyuiStatusState value) { foreach (var state in Enum.GetNames<XyuiStatusState>()) Classes.Remove($"xyui-status-dot-{state.ToString().ToLowerInvariant()}"); Classes.Add($"xyui-status-dot-{value.ToString().ToLowerInvariant()}"); }
}
