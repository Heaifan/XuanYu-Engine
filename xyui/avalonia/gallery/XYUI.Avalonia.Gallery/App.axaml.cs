using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Gallery;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        Styles.Add(XyuiTextStyles.Create());
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
