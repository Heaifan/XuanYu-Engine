using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiTextComponent : TextBlock
{
    protected XyuiTextComponent(string className)
    {
        Classes.Add("xyui-1-component");
        Classes.Add(className);
    }

    public abstract string CanonicalId { get; }
}

public abstract class XyuiTextSurface : Border
{
    protected readonly TextBlock TextPresenter = new();

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<XyuiTextSurface, string>(nameof(Text), "");

    protected XyuiTextSurface(string className)
    {
        Classes.Add("xyui-1-component");
        Classes.Add(className);
        TextPresenter.Classes.Add($"{className}-text");
        Child = TextPresenter;
    }

    public string Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public abstract string CanonicalId { get; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty) TextPresenter.Text = change.GetNewValue<string>();
    }
}
