using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticDetailPanel : Border
{
    DiagnosticElementSnapshot _snapshot = null!;
    Func<string, Task> _copy = _ => Task.CompletedTask;
    Action _closed = () => { };
    public Control DragSurface => Header;

    public DiagnosticDetailPanel() => InitializeComponent();

    public DiagnosticDetailPanel(DiagnosticElementSnapshot snapshot, Func<string, Task> copy, Action closed)
    {
        _snapshot = snapshot; _copy = copy; _closed = closed; InitializeComponent();
        Title.Text = snapshot.Identity.IsMapped ? snapshot.Identity.DisplayIndex :
            snapshot.InstanceName != "N/A" ? snapshot.InstanceName : snapshot.ComponentType; Show("元素");
        Element.Click += (_, _) => Show("元素"); Layout.Click += (_, _) => Show("布局");
        State.Click += (_, _) => Show("状态"); Style.Click += (_, _) => Show("样式");
        Close.Click += (_, _) => _closed(); CopyAi.Click += async (_, _) => await _copy(DiagnosticReportFormatter.FormatAi(_snapshot));
    }

    void Show(string page)
    {
        Content.Text = page switch
        {
            "元素" => $"XYUI索引：{_snapshot.Identity.DisplayIndex}\n组件名称：{_snapshot.ComponentType}\n视觉路径：\n{_snapshot.VisualPath}",
            "布局" => $"实际尺寸：{_snapshot.ActualSize}\n期望尺寸：{_snapshot.DesiredSize}\n位置：{_snapshot.Position}\n外边距：{_snapshot.Margin}\n内边距：{_snapshot.Padding}",
            "状态" => $"可见：{_snapshot.Visible}\n启用：{_snapshot.Enabled}\n获得焦点：{_snapshot.Focused}\n鼠标悬停：{_snapshot.PointerOver}\n按下：{_snapshot.Pressed}\n选中：{_snapshot.Selected}\n展开：{_snapshot.Expanded}",
            _ => "样式名称：不可用\n模板名称：不可用\n字体：不可用\n字号：不可用\n前景色：不可用\n背景色：不可用"
        };
    }
}
