using XuanYu.Engine.Editor.Windows.Panels.Viewport.Input;

namespace XuanYu.Engine.Editor.Windows.Shell.Input;

public sealed class EditorViewportInputState
{
    public int LastPointerX { get; set; }
    public int LastPointerY { get; set; }
    public WindowsViewportInputTranslator? Translator { get; set; }
}
