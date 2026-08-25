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

// Batch 01 运行时测试宿主：注入主题资源与家族样式，提供 Edge 定位、真实鼠标悬停与 token 取色辅助。
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

    // 真实 headless 指针悬停：先移出再移入目标中心，驱动原生 :pointerover 伪类。
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
