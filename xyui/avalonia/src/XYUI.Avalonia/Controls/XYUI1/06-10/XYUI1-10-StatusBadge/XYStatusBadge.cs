using Avalonia;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XyuiStatusState { Success, Warning, Error, Info, Neutral }

public sealed class XYStatusBadge : XyuiVectorTextSurface
{
    public const double DotSize = 8;
    public static readonly StyledProperty<XyuiStatusState> StateProperty =
        AvaloniaProperty.Register<XYStatusBadge, XyuiStatusState>(nameof(State), XyuiStatusState.Neutral);

    public XYStatusBadge() : base("xyui-status-badge", XyuiVectorIcon.StatusDot, XyuiVectorMarkPlacement.Inline)
    {
        VectorMark.Width = DotSize; VectorMark.Height = DotSize;
        ApplyState(State);
    }
    public override string CanonicalId => "XYUI-1-10";
    public XyuiStatusState State { get => GetValue(StateProperty); set { SetValue(StateProperty, value); ApplyState(value); } }
    void ApplyState(XyuiStatusState value)
    {
        foreach (var state in Enum.GetNames<XyuiStatusState>())
        {
            var name = state.ToLowerInvariant(); Classes.Remove($"xyui-status-{name}"); TextPresenter.Classes.Remove($"xyui-status-text-{name}"); VectorMark.Classes.Remove($"xyui-status-mark-{name}");
        }
        var current = value.ToString().ToLowerInvariant(); Classes.Add($"xyui-status-{current}"); TextPresenter.Classes.Add($"xyui-status-text-{current}"); VectorMark.Classes.Add($"xyui-status-mark-{current}");
    }
}
