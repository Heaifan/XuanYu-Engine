using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4/D4-F1：地图属性输入表单响应式（EditableFormRow：内容宽 <360 整组上下）。
// 只读资产摘要保持单行双列不参与切换；标准/窄模式共享同一 UiVm 状态。
// 模式判定纯逻辑在 MapEditorLayoutModel；标准/紧凑共享同一 UiVm 状态。
public partial class MapPagePanel : UserControl
{
    public MapPagePanel()
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

    async void CopyMapId_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not UiVm vm || string.IsNullOrEmpty(vm.MapIdText)) return;
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            Debug.WriteLine("[MapPagePanel] clipboard unavailable");
            return;
        }
        await clipboard.SetTextAsync(vm.MapIdText); // 复制内容必须是未经截断的完整 MapId
    }
}
