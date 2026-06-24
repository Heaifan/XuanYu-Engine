using FluidWarfare.Editor.Windows.Panels.Viewport.Input;

namespace FluidWarfare.Editor.Windows.Shell.Input;

public sealed class EditorViewportInputState
{
    public int LastPointerX { get; set; }
    public int LastPointerY { get; set; }
    public WindowsViewportInputTranslator? Translator { get; set; }
}
