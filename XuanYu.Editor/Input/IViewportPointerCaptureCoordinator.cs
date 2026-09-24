namespace XuanYu.Editor.Input;

public interface IViewportPointerCaptureCoordinator
{
    void Capture(long pointerId, GestureOwner owner);
    void Release(long pointerId, GestureOwner owner);
}
