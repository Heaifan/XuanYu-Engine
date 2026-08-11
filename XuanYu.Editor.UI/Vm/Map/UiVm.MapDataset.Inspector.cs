namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string InspectorSelectionTitle => SelectedDataset?.Name ?? SelectionTitle;
    public string InspectorSelectionSubtitle => SelectedDataset is { } item ? item.TypeIdText : SelectionSubtitle;
    public bool HasInspectorSelection => HasSelectedDataset || HasSelection;
    public bool IsInspectorEmpty => !HasInspectorSelection;
}
