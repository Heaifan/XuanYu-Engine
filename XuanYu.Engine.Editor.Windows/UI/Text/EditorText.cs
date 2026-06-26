namespace XuanYu.Engine.Editor.Windows.UI.Text;

/// <summary>编辑器 UI 文本集中管理。默认中文，后续可升级为资源文件多语言。新增 UI 文本必须通过此类，禁止散落硬编码。</summary>
internal static class EditorText
{
    // ─── 菜单 ─────────────────────────────────────
    public const string File = "文件";
    public const string Edit = "编辑";
    public const string View = "视图";
    public const string Window = "窗口";
    public const string Help = "帮助";

    // ─── 命令 ─────────────────────────────────────
    public const string Undo = "撤销";
    public const string Redo = "重做";
    public const string Save = "保存";
    public const string ResetLayout = "重置布局";
    public const string Play = "运行";
    public const string Stop = "停止";

    // ─── 工具 ─────────────────────────────────────
    public const string Select = "选择";
    public const string Move = "移动";
    public const string Rotate = "旋转";
    public const string Scale = "缩放";
    public const string GlobalLocal = "世界/本地";
    public const string Snap = "吸附";
    public const string Grid = "网格";
}
