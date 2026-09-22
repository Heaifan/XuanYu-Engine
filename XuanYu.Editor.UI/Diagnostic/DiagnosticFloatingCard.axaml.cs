using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticFloatingCard : Border
{
    DiagnosticElementSnapshot _snapshot = null!;
    Func<string, Task> _copy = _ => Task.CompletedTask;
    public event Action? Closed;
    public event Action? Expanded;
    public event Action? PinToggled;
    public Control DragSurface => Header;

    public DiagnosticFloatingCard() => InitializeComponent();

    public DiagnosticFloatingCard(DiagnosticElementSnapshot snapshot, Func<string, Task> copy, bool locked = true)
    {
        InitializeComponent(); _snapshot = snapshot; _copy = copy;
        Title.Text = snapshot.Identity.IsMapped ? snapshot.Identity.DisplayIndex :
            snapshot.InstanceName != "N/A" ? snapshot.InstanceName : snapshot.ComponentType;
        Locked.IsVisible = locked; Width = locked ? 330 : 250;
        Pin.Content = locked ? "解除" : "锁定"; CopyAi.Content = locked ? "一键复制给 AI" : "复制";
        CopyIndex.IsVisible = locked; CopySummary.IsVisible = locked; Expand.IsVisible = locked; Close.IsVisible = locked;
        Identity.Text = $"实例名称：{snapshot.InstanceName}\n组件来源：{snapshot.Identity.Source}";
        Details.Text = $"调试编号：{snapshot.DebugId}\n实际尺寸：{snapshot.ActualSize}";
        CopyIndex.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatIndex(_snapshot));
        CopySummary.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatSummary(_snapshot));
        CopyAi.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatAi(_snapshot));
        Expand.Click += (_, _) => Expanded?.Invoke(); Close.Click += (_, _) => Closed?.Invoke();
        Pin.Click += (_, _) => PinToggled?.Invoke();
    }
}
