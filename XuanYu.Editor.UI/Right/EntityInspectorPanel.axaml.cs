using Avalonia.Controls;
using Avalonia.Input;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class EntityInspectorPanel : UserControl
{
    string _nameEditStart = "";

    public EntityInspectorPanel()
    {
        InitializeComponent();
        NameBox.GotFocus += (_, _) =>
        {
            _nameEditStart = NameBox.Text ?? "";
            (DataContext as UiVm)?.BeginInspectorEntityNameEdit();
        };
        NameBox.KeyDown += NameBox_KeyDown;
        NameBox.LostFocus += (_, _) => CommitName();
        PositionProperty.ValueChanged += (_, _) => CommitVector("位置", PositionProperty);
        RotationProperty.ValueChanged += (_, _) => CommitVector("旋转", RotationProperty);
        ScaleProperty.ValueChanged += (_, _) => CommitVector("缩放", ScaleProperty);
    }

    void NameBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { CommitName(); e.Handled = true; }
        else if (e.Key == Key.Escape) { NameBox.Text = _nameEditStart; e.Handled = true; }
    }

    void CommitName() => (DataContext as UiVm)?.CommitInspectorEntityName();

    void CommitVector(string group, XYVectorProperty property) =>
        (DataContext as UiVm)?.CommitInspectorVector(group, property.X, property.Y, property.Z);
}
