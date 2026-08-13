using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace XYUI.Avalonia.Gallery;

public sealed partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        MainWindow? mainWindow = null;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;
        }
        else
            throw new InvalidOperationException("Gallery requires a desktop Avalonia lifetime.");
        base.OnFrameworkInitializationCompleted();
        if (mainWindow is not null)
        {
            mainWindow.Show();
        }
    }
}
