using XuanYu.Editor.MapEditing;
using XuanYu.Editor.Mode;
using XuanYu.Editor.Workspace;

namespace XuanYu.Editor.UI;

public sealed record UiProgressProjection(bool IsActive, double Percentage, string Message);

public sealed record UiNotificationSnapshot(UiNotificationLevel Level, string Text, int Count)
{
    public static UiNotificationSnapshot Empty { get; } = new(UiNotificationLevel.Info, "", 1);
}

public sealed record EditorApplicationState(
    EditorModeId Mode,
    EditorWorkspaceId Workspace,
    EditorContextId Context,
    EditorToolSnapshot Tool,
    EditorSelectionSnapshot Selection,
    EditorInteractionSnapshot Interaction,
    string? LastAuthoringTool,
    bool IsAuthoring,
    UiProgressProjection Progress,
    UiNotificationSnapshot Notification)
{
    public string ContextLabel => Context == EditorContextId.Terrain ? "地形" : "区域";
    public string ActiveToolText => Tool.ActiveToolText;
    public string ToolbarLabel => Context == EditorContextId.Terrain ? "地形"
        : IsAuthoring ? ActiveToolText : LastAuthoringTool switch
        {
            "地图标记" => "点标记",
            "区域面" => "区域",
            _ => LastAuthoringTool ?? "绘制"
        };
    public string StatusLabel
    {
        get
        {
            var workspace = EditorWorkspaceDefinitions.Resolve(Workspace).DisplayName;
            var context = Mode == EditorModeId.Manage ? $"管理模式 · 编辑目标：{workspace}" : workspace;
            return $"{context} · 工具：{ActiveToolText}";
        }
    }
}

public static class EditorApplicationStateProjection
{
    public static EditorApplicationState Create(
        EditorModeId mode, EditorWorkspaceDefinition workspace,
        EditorContextSnapshot context, EditorToolSnapshot tool,
        EditorSelectionSnapshot selection, EditorInteractionSnapshot interaction,
        AuthoringInputSnapshot authoring, string? lastAuthoringTool, double progressPercentage,
        string progressMessage, bool isProgressActive, UiNotificationSnapshot notification) =>
        new(mode, workspace.Id, context.Context, tool, selection, interaction,
            lastAuthoringTool, authoring.IsActive,
            new(isProgressActive, progressPercentage, progressMessage), notification);
}
