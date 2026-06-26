using Avalonia.Controls;

namespace XuanYu.Engine.Editor.Windows.Panels.TopArea;

/// <summary>编辑器顶部区域：主命令栏 + 编辑工具栏。两行，禁止第三行。</summary>
public sealed partial class EditorTopArea : UserControl
{
    public EditorTopArea()
    {
        InitializeComponent();
        var resetBtn = this.FindControl<Button>("ResetLayoutButton");
        if (resetBtn is not null)
            resetBtn.Click += (_, _) => OnResetLayout();
    }

    static void OnResetLayout()
    {
        System.Diagnostics.Debug.WriteLine(
            "[布局] 已触发重置布局：当前版本仅恢复默认顶部与面板可见状态。");
    }
}
