using System.Windows.Input;
using XuanYu.Editor.Workspace;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isGeometryEditingActive;
    ICommand? _toggleGeometryEditingCommand;
    ICommand? _toggleFeatureEditingCommand;

    public bool IsFeatureEditingActive
    {
        get => IsRegionWorkspace;
        set
        {
            if (value == IsRegionWorkspace) return;
            if (value) SwitchWorkspace(EditorWorkspaceId.RegionEditor);
            else SwitchWorkspace(EditorWorkspaceId.MapEditor);
            OnPropertyChanged();
        }
    }

    public ICommand ToggleFeatureEditingCommand => _toggleFeatureEditingCommand ??=
        new RelayCommand(_ => IsFeatureEditingActive = !IsFeatureEditingActive);

    public bool IsGeometryEditingActive
    {
        get => _isGeometryEditingActive;
        set
        {
            if (_isGeometryEditingActive == value) return;
            _isGeometryEditingActive = value;
            OnPropertyChanged();
            RaiseMapGeometryBindings();
        }
    }

    public ICommand ToggleGeometryEditingCommand => _toggleGeometryEditingCommand ??=
        new RelayCommand(_ => IsGeometryEditingActive = !IsGeometryEditingActive);
}
