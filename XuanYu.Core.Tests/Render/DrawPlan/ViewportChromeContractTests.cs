using System.IO;

namespace XuanYu.Core.Tests.Render;

// F3-D1：视口黑边合同测试（计划 11.1）——XAML 防退化：
// 视口外层无深色粗边框/大圆角/大 Padding；Native Host 贴边；白色占位卡片已删除。
public sealed class ViewportChromeContractTests
{
    static string UiFile(string path)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var full = Path.Combine(root, "XuanYu.Editor.UI", path);
        Assert.True(File.Exists(full), $"UI 文件缺失：{full}");
        return File.ReadAllText(full);
    }

    [Fact]
    public void Viewport_container_has_no_dark_frame()
    {
        var xaml = UiFile(Path.Combine("Viewport", "Vulkan", "VulkanViewport.axaml"));
        Assert.DoesNotContain("#0b1220", xaml);       // 深蓝黑背景已删除
        Assert.DoesNotContain("#31405d", xaml);       // 深色边框已删除
        Assert.DoesNotContain("CornerRadius", xaml);  // 无圆角卡片
    }

    [Fact]
    public void Main_layout_center_has_no_dark_card()
    {
        var xaml = UiFile(Path.Combine("Root", "UiRoot.axaml"));
        Assert.DoesNotContain("#101827", xaml);       // 深色背景已删除
        Assert.DoesNotContain("Padding=\"18\"", xaml); // 大 Padding 已删除
        Assert.DoesNotContain("CornerRadius=\"8\"", xaml);
        Assert.DoesNotContain("BoxShadow", xaml);
    }

    [Fact]
    public void View_gizmo_white_card_replaced()
    {
        // F3-F1：Avalonia 覆盖层已删除（airspace 遮挡），导航 Gizmo 移入 Vulkan Overlay Pass。
        var uiRoot = UiFile(Path.Combine("Root", "UiRoot.axaml"));
        Assert.DoesNotContain("ViewGizmo", uiRoot);
        var dir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Viewport");
        Assert.False(File.Exists(Path.Combine(dir, "ViewGizmo.axaml")), "ViewGizmo.axaml 应已删除");
        Assert.False(File.Exists(Path.Combine(dir, "ViewNavigationGizmo.cs")), "Avalonia Gizmo 控件应已删除");
        Assert.True(File.Exists(Path.Combine(dir, "ViewNavigationGizmo.Layout.cs")), "投影纯数学应保留（命中复用）");
        Assert.True(File.Exists(Path.Combine(dir, "ViewNavigationGizmo.HitTest.cs")), "命中纯数学应保留");
    }

    [Fact]
    public void Native_host_stretches_to_fill()
    {
        var xaml = UiFile(Path.Combine("Viewport", "Vulkan", "VulkanViewport.axaml"));
        Assert.Contains("<local:VulkanNativeHost/>", xaml);
    }
}
