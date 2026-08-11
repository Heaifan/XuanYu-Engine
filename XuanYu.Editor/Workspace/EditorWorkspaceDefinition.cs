namespace XuanYu.Editor.Workspace;

// EDITOR-A-R1：只描述 Workspace 布局上下文身份，不创建 UI 控件。
public sealed record EditorWorkspaceDefinition(
    EditorWorkspaceId Id,
    string DisplayName,
    string ToolbarSlot,
    string LeftPanelSlot,
    string MainContentSlot,
    string RightPanelSlot,
    EditorWorkspaceTool DefaultTool);
