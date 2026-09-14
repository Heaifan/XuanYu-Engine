using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class MapEditorPanel : UserControl
{
    public event Action<string>? TabChanged;
    public string SelectedTabId => (DataContext as UiVm)?.ExpandedInspectorSection switch
    {
        InspectorSectionId.Assets => "data", InspectorSectionId.Environment => "environment", _ => "base"
    };

    public MapEditorPanel() => InitializeComponent();

    public void SelectTab(string id)
    {
        if (DataContext is not UiVm vm) return;
        var section = id switch { "data" => InspectorSectionId.Assets,
            "environment" => InspectorSectionId.Environment, _ => InspectorSectionId.Basic };
        vm.ToggleInspectorSectionCommand.Execute(section);
        TabChanged?.Invoke(id);
    }
}
