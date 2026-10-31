namespace XuanYu.Editor.Workspace;

// EDITOR-A-R1：交给未来 Shell/UI 执行的无副作用切换合同。
public sealed record EditorWorkspaceTransition(
    EditorWorkspaceDefinition PreviousWorkspace,
    EditorWorkspaceDefinition CurrentWorkspace,
    bool Changed,
    bool IsLeave,
    bool EndsTemporaryToolState,
    bool PreservesWorldContext,
    bool PreservesCameraContext,
    bool PreservesSelection)
{
    public EditorWorkspaceTool NextTool => CurrentWorkspace.DefaultTool;
}
