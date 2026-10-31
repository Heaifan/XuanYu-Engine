using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3TabDividerContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fixture;
    public XYUI3TabDividerContractTests(XyuiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Tabs_do_not_render_the_vertical_divider()
    {
        _fixture.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var tabs = new XYTabs(new XYTab { Id = "project", Label = "项目", IsClosable = false },
                new XYTab { Id = "file", Label = "文件", IsClosable = false }) { Width = 220 };
            var window = XyuiBatchTestHost.Show(tabs);
            Assert.DoesNotContain(tabs.GetVisualDescendants().OfType<Border>(),
                border => border.Classes.Contains("xyui-tab-divider"));
            window.Close();
        });
    }
}
