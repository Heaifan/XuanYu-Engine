namespace FluidWarfare.Editor.Windows.Shell.Input.Picking;

/// <summary>PickRoute 的结果。Shell 据此应用选择和地面标记。</summary>
public sealed record EditorPickInputResult(
    bool SelectionChanged,
    bool GroundCursorShown,
    bool PlacementCompleted);
