using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1（纠偏 v2）：地图属性输入表单——EditableFormRow 方向切换
// （内容宽 <360 整组上下）。只控制输入表单方向，不参与面板密度（MapEditorLayoutModel）。
public partial class MapFormPanel : UserControl
{
    public MapFormPanel()
    {
        InitializeComponent();
        SizeChanged += (_, _) => ApplyMode();
        ApplyMode();
    }

    void ApplyMode()
    {
        var narrow = EditableFormLayoutModel.ModeFor(Bounds.Width) == EditableFormMode.Narrow;
        PropsWide.IsVisible = !narrow;
        PropsNarrow.IsVisible = narrow;
    }
}
