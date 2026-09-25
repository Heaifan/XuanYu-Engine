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
        InitializeComponent();
        ApplySnapshot(snapshot, copy, locked);
        CopyAi.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatAi(_snapshot));
        Expand.Click += (_, _) => Expanded?.Invoke(); Close.Click += (_, _) => Closed?.Invoke();
        Pin.Click += (_, _) => PinToggled?.Invoke();
    }

    public void UpdateSnapshot(DiagnosticElementSnapshot snapshot, Func<string, Task> copy, bool locked)
    {
        ApplySnapshot(snapshot, copy, locked);
    }

    void ApplySnapshot(DiagnosticElementSnapshot snapshot, Func<string, Task> copy, bool locked)
    {
        _snapshot = snapshot; _copy = copy;
        Title.Text = snapshot.TargetDisplayName;
        Locked.IsVisible = locked; Width = locked ? 380 : 280;
        Pin.Content = locked ? "解除" : "锁定"; CopyAi.Content = "一键复制给 AI";
        Expand.IsVisible = locked; Close.IsVisible = locked;
        Identity.Text = $"实例名称：{snapshot.InstanceName}\n组件来源：{snapshot.Identity.Source}\n" +
            $"组件类型：{Value(snapshot.Identity.ComponentName, snapshot.ComponentType)}\n" +
            $"组件索引：{Value(snapshot.Identity.Number)}\n组件编号：{Value(snapshot.Identity.CatalogId)}";
        Details.Text = $"调试编号：{snapshot.DebugId}\n所属区域：{Value(snapshot.Probe.ParentDebugId)}\n" +
            $"实际尺寸：{snapshot.ActualSize}";
    }

    static string Value(string value, string fallback = "—") =>
        string.IsNullOrWhiteSpace(value) || value == "N/A" ? fallback : value;
}
