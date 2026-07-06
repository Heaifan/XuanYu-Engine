using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace XuanYu.Editor.UI;

public sealed class App : Application
{
    public override void Initialize()
    {
        RequestedThemeVariant = ThemeVariant.Light;
        Styles.Add(new FluentTheme());
        Styles.Add(new StyleInclude(new Uri("avares://XuanYu.Editor.UI/"))
        {
            Source = new Uri("avares://XuanYu.Editor.UI/Ui.axaml")
        });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = new UiVm();
            desktop.MainWindow = new UiWin
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
