using Xunit;
using XYUI.Avalonia.Gallery;
using System.IO;

namespace XYUI.Avalonia.Tests;

// G0 · 折叠章节导航的最小合同测试（仅验证导航行为，不依赖 UI 渲染）
public class NavigationCollapseTests
{
    [Fact]
    public void XYUI1_Collapsed_By_Default_And_XYUI2_Expanded()
    {
        var vm = new XYUI1DocumentationViewModel();
        Assert.False(vm.IsXYUI1Expanded);   // XYUI-1 FROZEN → 默认折叠
        Assert.True(vm.IsXYUI2Expanded);    // XYUI-2 当前工作区 → 默认展开
    }

    [Fact]
    public void Collapsing_A_Module_Section_Does_Not_Change_Current_Page()
    {
        var vm = new XYUI1DocumentationViewModel();
        var before = vm.SelectedDocument;
        Assert.NotNull(before);

        // 折叠 XYUI-1：当前页不得被踢走
        vm.IsXYUI1Expanded = false;
        Assert.Same(before, vm.SelectedDocument);

        // 选中 XYUI-2 组件后再折叠 XYUI-2，页面应保持不变
        vm.Select("XYUI-2-01");
        var page = vm.SelectedDocument;
        Assert.NotNull(page);
        vm.IsXYUI2Expanded = false;
        Assert.Same(page, vm.SelectedDocument);
    }

    [Fact]
    public void NavSection_Uses_Vector_Chevron_Not_Unicode_And_No_Card()
    {
        var axaml = ReadViewAxaml();
        // G0-R1：弃用 Accordion/Card，必须是矢量 Path chevron，且不得出现 Unicode 箭头字符
        Assert.DoesNotContain("<Expander", axaml);
        Assert.Contains("<Path", axaml);
        foreach (var ch in new[] { '▸', '▾', '→', '←', '↓', '↑' })
            Assert.DoesNotContain(ch.ToString(), axaml);
    }

    static string ReadViewAxaml()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var p = Path.Combine(dir.FullName, "xyui", "avalonia", "gallery",
                "XYUI.Avalonia.Gallery", "Views", "XYUI1DocumentationView.axaml");
            if (File.Exists(p)) return File.ReadAllText(p);
            dir = dir.Parent;
        }
        throw new FileNotFoundException("XYUI1DocumentationView.axaml not found");
    }
}
