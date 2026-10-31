namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string? DiagnosticFeatureModuleId => FeatureDiagnosticIds.FeatureModule(InspectorIdentity);
    public string? DiagnosticFeatureBasicId => FeatureDiagnosticIds.Section(InspectorIdentity, "BASIC");
    public string? DiagnosticFeatureGeometryId => FeatureDiagnosticIds.Section(InspectorIdentity, "GEOMETRY");
    public string? DiagnosticFeatureStateId => FeatureDiagnosticIds.Section(InspectorIdentity, "STATE");
    public string? DiagnosticFeatureRelationsId => FeatureDiagnosticIds.Section(InspectorIdentity, "RELATIONS");
    void RaiseDiagnosticIdentityBindings()
    {
        OnPropertyChanged(nameof(DiagnosticFeatureModuleId));
        OnPropertyChanged(nameof(DiagnosticFeatureBasicId));
        OnPropertyChanged(nameof(DiagnosticFeatureGeometryId));
        OnPropertyChanged(nameof(DiagnosticFeatureStateId));
        OnPropertyChanged(nameof(DiagnosticFeatureRelationsId));
    }
}
