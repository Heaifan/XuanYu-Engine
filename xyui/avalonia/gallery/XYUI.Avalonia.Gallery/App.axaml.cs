using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Theme;
using XYUI.Avalonia.Typography;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        Resources.MergedDictionaries.Add(XyuiVectorIcons.CreateResources());
        Styles.Add(XyuiTextStyles.Create());
        Styles.Add(XyuiShapeStyles.Create());
        Styles.Add(XyuiInteractionStyles.Create());
        Styles.Add(XyuiControlStyles.Create());
        Styles.Add(XyuiComponentStyles.Create());
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
