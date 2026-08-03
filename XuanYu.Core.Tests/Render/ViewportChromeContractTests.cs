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
        var xaml = UiFile(Path.Combine("Viewport", "ViewGizmo.axaml"));
        Assert.DoesNotContain("#dce6f2", xaml);        // 白色卡片背景已删除
        Assert.DoesNotContain("CornerRadius=\"10\"", xaml);
        Assert.DoesNotContain("BoxShadow", xaml);
        Assert.DoesNotContain("Button", xaml);         // 3×3 按钮网格已删除
        Assert.Contains("ViewNavigationGizmo", xaml);  // 替换为导航 Gizmo
    }

    [Fact]
    public void Native_host_stretches_to_fill()
    {
        var xaml = UiFile(Path.Combine("Viewport", "Vulkan", "VulkanViewport.axaml"));
        Assert.Contains("<local:VulkanNativeHost/>", xaml);
    }
}
