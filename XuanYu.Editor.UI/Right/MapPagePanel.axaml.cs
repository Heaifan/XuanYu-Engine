using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决）：地图页——紧凑模式（<320 整组上下）与 MapId 完整值复制。
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
        var compact = MapEditorLayoutModel.ModeFor(Bounds.Width) == MapEditorDensityMode.Compact;
        PropsWide.IsVisible = !compact;
        PropsNarrow.IsVisible = compact;
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
