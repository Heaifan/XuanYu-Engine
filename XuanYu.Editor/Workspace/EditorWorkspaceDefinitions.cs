namespace XuanYu.Editor.Workspace;

// EDITOR-A-R1：两个首批 Workspace 的不可变定义。
public static class EditorWorkspaceDefinitions
{
    public static EditorWorkspaceDefinition MapEditor { get; } = new(
        EditorWorkspaceId.MapEditor,
        "地图编辑",
        "map-editor-toolbar",
        "map-editor-left",
        "map-editor-main",
        "map-editor-right",
        EditorWorkspaceTool.Select);

    public static EditorWorkspaceDefinition RegionEditor { get; } = new(
        EditorWorkspaceId.RegionEditor,
        "区域编辑",
        "region-editor-toolbar",
        "region-editor-left",
        "region-editor-main",
        "region-editor-right",
        EditorWorkspaceTool.Select);

    public static EditorWorkspaceDefinition RoadEditor { get; } = new(
        EditorWorkspaceId.RoadEditor, "道路编辑", "road-editor-toolbar", "road-editor-left", "road-editor-main", "road-editor-right", EditorWorkspaceTool.Select);

    public static IReadOnlyList<EditorWorkspaceDefinition> All { get; } =
        [MapEditor, RegionEditor, RoadEditor];

    public static EditorWorkspaceDefinition Resolve(EditorWorkspaceId id) => id switch
    {
        EditorWorkspaceId.MapEditor => MapEditor,
        EditorWorkspaceId.RegionEditor => RegionEditor,
        EditorWorkspaceId.RoadEditor => RoadEditor,
        _ => throw new ArgumentOutOfRangeException(nameof(id), id, "未知编辑器 Workspace。")
    };
}
