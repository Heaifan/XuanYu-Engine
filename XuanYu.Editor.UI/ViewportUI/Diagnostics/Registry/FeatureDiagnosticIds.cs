namespace XuanYu.Editor.UI;

public static class FeatureDiagnosticIds
{
    public const string AreaTop = "XYE.AREA.TOP";
    public const string AreaLeft = "XYE.AREA.LEFT";
    public const string AreaCenter = "XYE.AREA.CENTER";
    public const string AreaRight = "XYE.AREA.RIGHT";
    public const string AreaBottom = "XYE.AREA.BOTTOM";
    public const string Menu = "XYE.MENU";
    public const string ContextToolbar = "XYE.CONTEXT_TOOLBAR";
    public const string ProjectTree = "XYE.PROJECT_TREE";
    public const string Viewport = "XYE.VIEWPORT";
    public const string Inspector = "XYE.INSPECTOR";
    public const string LayerDock = "XYE.LAYER_DOCK";
    public const string LogPanel = "XYE.LOG_PANEL";
    public static IReadOnlyList<string> ModuleIds { get; } =
        [Menu, ContextToolbar, ProjectTree, Viewport, Inspector, LayerDock, LogPanel];
    public static string? FeatureModule(InspectorObjectKind kind) => kind switch
    {
        InspectorObjectKind.Road => "XYE.INSPECTOR.ROAD",
        InspectorObjectKind.Region => "XYE.INSPECTOR.REGION",
        _ => null
    };
    public static string? Section(InspectorObjectKind kind, string section) =>
        FeatureModule(kind) is { } module ? $"{module}.{section}" : null;
}
