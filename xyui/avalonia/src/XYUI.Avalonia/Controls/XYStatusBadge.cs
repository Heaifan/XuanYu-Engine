using Avalonia;

namespace XYUI.Avalonia.Controls;

public enum XyuiStatusState { Success, Warning, Error, Info, Neutral }

public sealed class XYStatusBadge : XyuiTextSurface
{
    public static readonly StyledProperty<XyuiStatusState> StateProperty =
        AvaloniaProperty.Register<XYStatusBadge, XyuiStatusState>(nameof(State), XyuiStatusState.Neutral);

    public XYStatusBadge() : base("xyui-status-badge") => ApplyState(State);
    public override string CanonicalId => "XYUI-1-10";
    public XyuiStatusState State { get => GetValue(StateProperty); set { SetValue(StateProperty, value); ApplyState(value); } }
    protected override string FormatText(string value) => $"●  {value}";
    void ApplyState(XyuiStatusState value)
    {
        foreach (var state in Enum.GetNames<XyuiStatusState>())
        {
            var name = state.ToLowerInvariant(); Classes.Remove($"xyui-status-{name}"); TextPresenter.Classes.Remove($"xyui-status-text-{name}");
        }
        var current = value.ToString().ToLowerInvariant(); Classes.Add($"xyui-status-{current}"); TextPresenter.Classes.Add($"xyui-status-text-{current}");
    }
}
