using System.Windows.Input;

namespace XuanYu.Editor.UI;

public enum InspectorSectionId { Basic, Geometry, Status, Relations, Position, Assets, Coordinates, Environment, Technical }

public sealed partial class UiVm
{
    InspectorSectionId? _expandedInspectorSection;
    InspectorObjectKind _sectionIdentity;
    ICommand? _toggleInspectorSectionCommand;

    public InspectorSectionId? ExpandedInspectorSection => _expandedInspectorSection;
    public ICommand ToggleInspectorSectionCommand => _toggleInspectorSectionCommand ??=
        new RelayCommand(value => ToggleInspectorSection(value));
    public bool IsInspectorBasicExpanded => IsInspectorSectionExpanded(InspectorSectionId.Basic);
    public bool IsInspectorGeometryExpanded => IsInspectorSectionExpanded(InspectorSectionId.Geometry);
    public bool IsInspectorStatusExpanded => IsInspectorSectionExpanded(InspectorSectionId.Status);
    public bool IsInspectorRelationsExpanded => IsInspectorSectionExpanded(InspectorSectionId.Relations);
    public bool IsInspectorPositionExpanded => IsInspectorSectionExpanded(InspectorSectionId.Position);
    public bool IsInspectorAssetsExpanded => IsInspectorSectionExpanded(InspectorSectionId.Assets);
    public bool IsInspectorCoordinatesExpanded => IsInspectorSectionExpanded(InspectorSectionId.Coordinates);
    public bool IsInspectorEnvironmentExpanded => IsInspectorSectionExpanded(InspectorSectionId.Environment);
    public bool IsInspectorTechnicalExpanded => IsInspectorSectionExpanded(InspectorSectionId.Technical);

    void ResetInspectorSectionsIfIdentityChanged()
    {
        var identity = ResolveInspectorSelection();
        if (_sectionIdentity == identity) return;
        _sectionIdentity = identity;
        if (_expandedInspectorSection is null) return;
        _expandedInspectorSection = null;
        RaiseInspectorSectionBindings();
    }

    void ToggleInspectorSection(object? value)
    {
        if (!TryInspectorSection(value, out var section)) return;
        _expandedInspectorSection = _expandedInspectorSection == section ? null : section;
        RaiseInspectorSectionBindings();
    }

    bool IsInspectorSectionExpanded(InspectorSectionId section) => _expandedInspectorSection == section;
    static bool TryInspectorSection(object? value, out InspectorSectionId section)
    {
        if (value is InspectorSectionId id) { section = id; return true; }
        if (value is string text && Enum.TryParse(text, true, out section)) return true;
        section = default; return false;
    }

    void RaiseInspectorSectionBindings()
    {
        OnPropertyChanged(nameof(ExpandedInspectorSection));
        OnPropertyChanged(nameof(IsInspectorBasicExpanded)); OnPropertyChanged(nameof(IsInspectorGeometryExpanded));
        OnPropertyChanged(nameof(IsInspectorStatusExpanded)); OnPropertyChanged(nameof(IsInspectorRelationsExpanded));
        OnPropertyChanged(nameof(IsInspectorPositionExpanded)); OnPropertyChanged(nameof(IsInspectorAssetsExpanded));
        OnPropertyChanged(nameof(IsInspectorCoordinatesExpanded)); OnPropertyChanged(nameof(IsInspectorEnvironmentExpanded));
        OnPropertyChanged(nameof(IsInspectorTechnicalExpanded));
    }
}
