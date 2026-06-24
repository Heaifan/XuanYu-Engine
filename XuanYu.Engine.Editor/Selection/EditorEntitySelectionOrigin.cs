namespace XuanYu.Engine.Editor.Selection;

/// <summary>
/// 选择命令的真实来源。程序同步界面不是选择命令，不应从此 Origin 进入。
/// </summary>
public enum EditorEntitySelectionOrigin
{
    /// <summary>用户点击世界层级树。</summary>
    WorldHierarchy,

    /// <summary>用户在 3D 视口点击单位。</summary>
    ViewportPicking,

    /// <summary>项目加载或状态恢复。</summary>
    SelectionRestore
}
