using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ReleaseRegionPointerCapture(EditorPointerEventKind terminal)
    {
        if (_releasingRegionPointerCapture ||
            ViewportInput.Router.State.Owner != GestureOwner.Region) return;
        var state = ViewportInput.Router.State;
        _releasingRegionPointerCapture = true;
        try
        {
            ViewportInput.Dispatch(new EditorPointerEvent(terminal, new(0, 0),
                EditorPointerButtons.None, EditorPointerModifiers.None, 0, state.PointerId,
                new("region-session"), 1));
        }
        finally { _releasingRegionPointerCapture = false; }
    }
}
