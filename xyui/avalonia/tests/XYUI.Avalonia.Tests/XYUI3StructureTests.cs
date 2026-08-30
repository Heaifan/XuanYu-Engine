using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3StructureTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3StructureTests(XyuiHeadlessFixture fx) => _fx = fx;
    [Fact] public void MenuBar_has_bottom_divider() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var bar = new XYMenuBar(new XYMenuBarItem { Label = "文件" }); Assert.Single(bar.GetVisualDescendants().OfType<XYSeparator>()); });
    [Fact] public void MenuBar_active_item_has_indicator() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var item = new XYMenuBarItem { Label = "视图", IsActive = true }; Assert.Contains("xyui-menu-active", item.Classes); Assert.Contains(item.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-menu-bar-indicator")); });
    [Fact] public void Menu_uses_shared_XYMenuItem() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var item = new XYMenuItem { Label = "打开" }; var menu = new XYMenu(item); Assert.Same(item, menu.Items.Single()); Assert.Single(menu.GetVisualDescendants().OfType<XYMenuItem>()); });
    [Fact] public void Menu_has_leading_label_shortcut_trailing_columns() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var item = new XYMenuItem { Label = "打开", Shortcut = "Ctrl+O", HasSubMenu = true }; Assert.Equal(4, Assert.IsAssignableFrom<Grid>(item.Child).ColumnDefinitions.Count); });
    [Fact] public void Menu_uses_existing_divider() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var menu = new XYMenu(XYMenu.Separator()); Assert.Single(menu.GetVisualDescendants().OfType<XYSeparator>()); });
    [Fact] public void Menu_uses_vector_chevron() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var item = new XYMenuItem { Label = "导出", HasSubMenu = true }; var icon = item.GetVisualDescendants().OfType<XYIcon>().Single(); Assert.Equal(XYUI.Avalonia.Vector.XyuiVectorIcon.ChevronRight, icon.Icon); });
    [Fact] public void ContextMenu_reuses_XYMenu_and_has_context_header() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var menu = new XYMenu(new XYMenuItem { Label = "定位" }); var context = new XYContextMenu { ContextType = "ENTITY", ContextName = "Infantry_023", Menu = menu }; Assert.Same(menu, context.Menu); Assert.Contains(context.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "Infantry_023"); });
    [Fact] public void ContextMenu_danger_group_is_visually_separated() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var context = new XYContextMenu { Menu = new XYMenu(XYMenu.Separator(), new XYMenuItem { Label = "删除", IsDestructive = true }) }; Assert.Contains(context.GetVisualDescendants().OfType<XYSeparator>(), _ => true); Assert.Contains(context.GetVisualDescendants().OfType<XYMenuItem>(), x => x.IsDestructive); });
    [Fact] public void SubMenu_reuses_XYMenu_and_XYMenuItem() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var parent = new XYMenu(new XYMenuItem { Label = "导出", HasSubMenu = true }); var child = new XYMenu(new XYMenuItem { Label = "导出图片" }); var submenu = new XYSubMenu { ParentMenu = parent, ChildMenu = child }; Assert.Same(parent, submenu.ParentMenu); Assert.Same(child, submenu.ChildMenu); Assert.Equal(2, submenu.GetVisualDescendants().OfType<XYMenu>().Count()); });
    [Fact] public void SubMenu_has_connector_and_anchor() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var submenu = new XYSubMenu { ParentMenu = new XYMenu(), ChildMenu = new XYMenu() }; Assert.Single(submenu.GetVisualDescendants().OfType<XYSubMenuConnector>()); Assert.Single(submenu.GetVisualDescendants().OfType<Ellipse>(), x => x.Classes.Contains("xyui-sub-menu-anchor")); });
    [Fact] public void Gallery_registers_batch02_pages() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var docs = XYUI3DocumentationCatalog.Build(); Assert.Equal(8, docs.Count); Assert.All(docs.Where(x => x.Id != "XYUI-3-3.04"), x => Assert.Contains("AWAITING USER VISUAL ACCEPTANCE", x.Acceptance)); Assert.All(docs, x => Assert.NotNull(x.PreviewFactory())); });
    [Fact] public void NavigationMenu_uses_compact_v2_dimensions() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var menu = XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.05"); Assert.IsType<XYNavigationMenu>(menu); Assert.Equal(246, menu.Width); Assert.Equal(32, XyuiComponentTokens.NavigationMenuItemHeight); Assert.Equal(20, XyuiComponentTokens.NavigationMenuGroupLabelHeight); Assert.Equal(14, XyuiComponentTokens.NavigationMenuIconSize); });
    [Fact] public void Sidebar_Rail_And_Tabs_use_compact_v2_surfaces() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); Assert.IsType<XYSidebar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.06")); Assert.IsType<XYNavigationRail>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.07")); Assert.IsType<XYTabs>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.08")); });
}
