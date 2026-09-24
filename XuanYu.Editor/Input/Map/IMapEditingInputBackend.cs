using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Map;

public interface IMapEditingInputBackend
{
    bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state);
    void Begin(ViewportGestureContext context);
    void Update(ViewportGestureContext context);
    void Commit(ViewportGestureContext context);
    void Cancel(ViewportCancellationContext context);
}
