namespace XuanYu.Editor.Input;

public interface IViewportInputSink
{
    void Handle(EditorPointerEvent pointer);
    void Handle(EditorKeyEvent key) { }
}
