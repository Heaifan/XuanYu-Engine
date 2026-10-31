using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public enum DiagnosticProbeMode
{
    Semantic,
    DeepVisual
}

public sealed record DiagnosticProbeResult(
    Visual DeepVisual,
    Control? SemanticTarget,
    string DebugId,
    string ParentDebugId,
    string ControlType,
    string Name,
    string Text,
    string RuntimeLocator,
    bool Visible,
    bool Enabled,
    Rect? Bounds,
    DiagnosticProbeMode ProbeMode);
