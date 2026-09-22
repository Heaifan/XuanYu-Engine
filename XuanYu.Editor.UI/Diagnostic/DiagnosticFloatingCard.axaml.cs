using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticFloatingCard : Border
{
    DiagnosticElementSnapshot _snapshot = null!;
    Func<string, Task> _copy = _ => Task.CompletedTask;
    public event Action? Closed;
    public event Action? Expanded;

    public DiagnosticFloatingCard() => InitializeComponent();

    public DiagnosticFloatingCard(DiagnosticElementSnapshot snapshot, Func<string, Task> copy, bool locked = true)
    {
        InitializeComponent(); _snapshot = snapshot; _copy = copy;
        Title.Text = snapshot.Identity.DisplayIndex;
        Locked.IsVisible = locked; Actions.IsVisible = locked; Width = locked ? 330 : 250;
        Identity.Text = $"实例名称：{snapshot.InstanceName}\n组件来源：{snapshot.Identity.Source}";
        Details.Text = $"调试编号：{snapshot.DebugId}\n实际尺寸：{snapshot.ActualSize}";
        CopyIndex.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatIndex(_snapshot));
        CopySummary.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatSummary(_snapshot));
        CopyAi.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatAi(_snapshot));
        Expand.Click += (_, _) => Expanded?.Invoke(); Close.Click += (_, _) => Closed?.Invoke();
    }
}
