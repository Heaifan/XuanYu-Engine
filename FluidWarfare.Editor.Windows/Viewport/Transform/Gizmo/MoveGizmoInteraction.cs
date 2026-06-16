namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的交互状态机。
/// Idle → Hovering → PointerDown → Dragging → Confirmed/Cancelled → Idle
/// </summary>
public sealed class MoveGizmoInteraction
{
    public MoveGizmoElement HoveredElement { get; private set; } = MoveGizmoElement.None;
    public MoveGizmoElement ActiveElement { get; private set; } = MoveGizmoElement.None;
    public bool IsDragging => ActiveElement != MoveGizmoElement.None;

    public void SetHover(MoveGizmoElement element) => HoveredElement = element;
    public void ClearHover() => HoveredElement = MoveGizmoElement.None;

    public bool TryBeginDrag(MoveGizmoElement element)
    {
        if (element == MoveGizmoElement.None) return false;
        ActiveElement = element;
        return true;
    }

    public void EndDrag()
    {
        ActiveElement = MoveGizmoElement.None;
    }
}
