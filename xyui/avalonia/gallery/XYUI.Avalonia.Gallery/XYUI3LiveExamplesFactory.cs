using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    public static Control? CreateLiveExamples(string id) => id switch
    {
        "XYUI-3-3.01" => CreateMenuBarLiveExamples(),
        "XYUI-3-3.02" => CreateMenuLiveExamples(),
        "XYUI-3-3.03" => CreateContextMenuLiveExamples(),
        "XYUI-3-3.04" => CreateSubMenuLiveExamples(),
        "XYUI-3-3.05" => CreateNavigationMenuLiveExamples(),
        "XYUI-3-3.06" => CreateSidebarLiveExamples(),
        "XYUI-3-3.07" => CreateNavigationRailLiveExamples(),
        "XYUI-3-3.08" => CreateTabsLiveExamples(),
        "XYUI-3-3.09" => CreateTabBarLiveExamples(),
        "XYUI-3-3.10" => CreateDockTabsLiveExamples(),
        "XYUI-3-3.11" => CreateBreadcrumbLiveExamples(),
        "XYUI-3-3.12" => CreateTreeNavigationLiveExamples(),
        "XYUI-3-3.13" => CreatePaginationLiveExamples(),
        "XYUI-3-3.14" => CreateStepsLiveExamples(),
        "XYUI-3-3.15" => CreateToolbarLiveExamples(),
        "XYUI-3-3.16" => CreateToolGroupLiveExamples(),
        "XYUI-3-3.17" => CreateCommandBarLiveExamples(),
        "XYUI-3-3.18" => CreateCommandPaletteLiveExamples(),
        "XYUI-3-3.19" => CreateBackForwardNavigationLiveExamples(),
        "XYUI-3-3.20" => CreateWorkspaceSwitcherLiveExamples(),
        "XYUI-3-3.21" => CreateViewSwitcherLiveExamples(),
        "XYUI-3-3.22" => CreateTableOfContentsLiveExamples(),
        "XYUI-3-3.23" => CreateBottomNavigationLiveExamples(),
        "XYUI-3-3.24" => CreateNavigationDrawerLiveExamples(),
        XYUI3GalleryCatalog.ContextToolbarId => CreateContextToolbarLiveExamples(),
        _ => null
    };

    public static Control? CreateComposition(string id) => id switch
    {
        "XYUI-3-3.01" => CreateMenuBarComposition(),
        "XYUI-3-3.02" => CreateMenuComposition(),
        "XYUI-3-3.03" => CreateContextMenuComposition(),
        "XYUI-3-3.04" => CreateSubMenuComposition(),
        "XYUI-3-3.05" => CreateNavigationMenuComposition(),
        "XYUI-3-3.06" => CreateSidebarComposition(),
        "XYUI-3-3.07" => CreateNavigationRailComposition(),
        "XYUI-3-3.08" => CreateTabsComposition(),
        "XYUI-3-3.09" => CreateTabBarComposition(),
        "XYUI-3-3.10" => CreateDockTabsComposition(),
        "XYUI-3-3.11" => CreateBreadcrumbComposition(),
        "XYUI-3-3.12" => CreateTreeNavigationComposition(),
        "XYUI-3-3.13" => CreatePaginationComposition(),
        "XYUI-3-3.14" => CreateStepsComposition(),
        "XYUI-3-3.15" => CreateToolbarComposition(),
        "XYUI-3-3.16" => CreateToolGroupComposition(),
        "XYUI-3-3.17" => CreateCommandBarComposition(),
        "XYUI-3-3.18" => CreateCommandPaletteComposition(),
        "XYUI-3-3.19" => CreateBackForwardNavigationComposition(),
        "XYUI-3-3.20" => CreateWorkspaceSwitcherComposition(),
        "XYUI-3-3.21" => CreateViewSwitcherComposition(),
        "XYUI-3-3.22" => CreateTableOfContentsComposition(),
        "XYUI-3-3.23" => CreateBottomNavigationComposition(),
        "XYUI-3-3.24" => CreateNavigationDrawerComposition(),
        XYUI3GalleryCatalog.ContextToolbarId => CreateContextToolbarComposition(),
        _ => null
    };

    internal static Border WrapCard(Control child, string? title = null)
    {
        var panel = new StackPanel { Spacing = 8 };
        if (!string.IsNullOrEmpty(title))
        {
            panel.Children.Add(new TextBlock { Text = title, Classes = { "xyui-text-label" } });
        }
        panel.Children.Add(child);
        return new Border
        {
            Classes = { "xyui-surface-panel" },
            Padding = new global::Avalonia.Thickness(14, 12),
            CornerRadius = new global::Avalonia.CornerRadius(6),
            Child = panel
        };
    }
}
