using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// Batch 01 Headless contract host: injects theme/family styles and provides layout,
// synthetic pointer, and token assertions. It is not a desktop or pixel-rendering gate.
internal static class XyuiBatchTestHost
{
    internal static Application Prepare()
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries());
        app.Resources.MergedDictionaries.Add(XYUI.Avalonia.Vector.XyuiVectorIcons.CreateResources());
        app.Styles.Add(XYUI.Avalonia.Interaction.XyuiInteractionStyles.Create());
        app.Styles.Add(XYUI.Avalonia.Controls.XyuiControlStyles.Create());
        app.Styles.Add(XYUI.Avalonia.Controls.XyuiComponentStyles.Create());
        return app;
    }

    internal static Window Show(Control content)
    {
        var window = new Window { Width = 480, Height = 220, Content = content };
        window.Show();
        content.ApplyStyling();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    // Headless synthetic pointer hover: move out, then into the target center,
    // to drive Avalonia's headless :pointerover pseudo-class.
    internal static void Hover(Window window, Control target)
    {
        window.MouseMove(new Point(-50, -50));
        var center = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)
                     ?? new Point(target.Bounds.Width / 2, target.Bounds.Height / 2);
        window.MouseMove(center);
        Dispatcher.UIThread.RunJobs();
    }

    internal static XyuiActionEdge Edge(Control host) =>
        host.GetVisualDescendants().OfType<XyuiActionEdge>().Single();

    internal static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

    internal static Color Token(string id, bool dark = false) =>
        XyuiColorTokens.All.Single(t => t.TokenId == id).ToColor(dark);
}
