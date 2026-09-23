namespace XuanYu.Editor.UI;

public enum CompositionSpikeResult
{
    Partial
}

public static class CompositionSpikeFinding
{
    public const CompositionSpikeResult Result = CompositionSpikeResult.Partial;
    public const string Technique = "Avalonia visual overlay over the existing Vulkan NativeControlHost";
    public const string NativeChildHwnd = "Present: Win32 WS_CHILD HWND";
    public const string Airspace = "Blocks reliable Avalonia overlay composition over the child HWND";
    public const string ExtraUiWindow = "Absent: no Popup, PopupRoot, or second Window is used by the spike";
}
