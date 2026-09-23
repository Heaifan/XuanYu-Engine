using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace XuanYu.Viewport.CompositionSpike;

public sealed class CompositionSpikeControl : UserControl
{
    readonly TextBlock _status;
    int _clicks;

    public CompositionSpikeControl()
    {
        AvaloniaXamlLoader.Load(this);
        _status = this.FindControl<TextBlock>("Status")!;
        var button = this.FindControl<Button>("OverlayButton")!;
        button.Click += (_, _) => SetStatus("Click=原生 Avalonia Button；次数=" + ++_clicks);
        button.PointerEntered += (_, _) => SetStatus("Hover=原生 Avalonia Button");
        button.PointerExited += (_, _) => SetStatus("Hover=离开；Click=" + _clicks);
        var surface = this.FindControl<CompositionSurfaceHost>("Surface")!;
        surface.StatusChanged += (_, text) => SetStatus(text);
    }

    void SetStatus(string text) => _status.Text = text;
}
