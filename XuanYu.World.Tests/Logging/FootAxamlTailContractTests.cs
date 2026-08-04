using System.IO;
using System.Linq;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Logging;

// MAP-A-R2-D3-F3：源码合同——AXAML 尾部安全区与控制器两阶段定位结构。
// 说明：仓库无 Avalonia Headless 基础设施（不引入新依赖），
// 几何级验证由真机验收（A4）承担；本测试锁定源码结构合同。
public sealed class FootAxamlTailContractTests
{
    static readonly string Axaml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Foot", "Foot.axaml"));

    [Fact]
    public void LogList_keeps_virtualizing_items_panel()
    {
        Assert.Contains("<ItemsPanelTemplate>", Axaml);
        Assert.Contains("VirtualizingStackPanel", Axaml);
    }

    [Fact]
    public void Tail_safe_zone_is_12_dips_in_items_panel()
    {
        Assert.Contains("Margin=\"0,0,0,12\"", Axaml);
    }

    [Fact]
    public void ListBox_padding_no_longer_carries_tail_gap()
    {
        Assert.DoesNotContain("Padding=\"0,0,0,8\"", Axaml);
    }
}
