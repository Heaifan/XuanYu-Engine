using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（G03）：检查器响应式双模式——以内容区域宽度为准（<360 整组上下）。
// 纯模式判定在 InspectorLayoutModel；两个 ItemsControl 绑定同一 InspectorFields（数据与顺序不变）。
public partial class InspectorPanel : UserControl
{
    public InspectorPanel()
    {
        InitializeComponent();
        SizeChanged += (_, _) => ApplyMode();
        ApplyMode();
    }

    void ApplyMode()
    {
        var narrow = InspectorLayoutModel.ModeFor(Bounds.Width) == InspectorFormMode.Narrow;
        WideFields.IsVisible = !narrow;
        NarrowFields.IsVisible = narrow;
    }
}
