using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class CompositionSpikeView : UserControl
{
    public CompositionSpikeView()
    {
        InitializeComponent();
        OverlayButton.PointerEntered += OnPointerEntered;
        OverlayButton.PointerExited += OnPointerExited;
    }

    void OnPointerEntered(object? sender, PointerEventArgs e) =>
        PointerState.Text = "Hover: Avalonia PointerEntered";

    void OnPointerExited(object? sender, PointerEventArgs e) =>
        PointerState.Text = "Hover: 已离开";

    void OnButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        PointerState.Text = "Click: Avalonia Button";
}
