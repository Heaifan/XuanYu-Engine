using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

// XYUI-2 组件目录一致性：identity.json、Catalog 映射、mapping.json Token 表与 Gallery
// 文档/预览保持单一事实源；05 的验收状态必须停留在待人工视觉验收。
[Collection("XyuiHeadless")]
public sealed class XYUI2ComponentReconcileTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2ComponentReconcileTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Component_pages_stay_pending_acceptance_with_canonical_identity()
    {
        var documents = XYUI2DocumentationCatalog.Build();
        Assert.Equal(24, documents.Count);
        Assert.All(documents, document => Assert.StartsWith(
            XYUI2DocumentationCatalog.PendingAcceptance, document.StatusText));
        var dropdown = documents.Single(x => x.Id == "XYUI-2-05");
        Assert.Equal("XY.DropDownButton · XYDropDownButton", dropdown.CanonicalDisplay);
        Assert.All(dropdown.Usages, usage => Assert.Contains("XYDropDownButton", usage));
        Assert.Contains(dropdown.Tokens, token => token.Name == "XY.DropDownButton.Height");
        Assert.Equal(["按钮", "图标按钮", "切换按钮", "分裂按钮", "下拉按钮", "复选框", "单选按钮", "开关", "文本输入框", "数值输入框", "滑块", "组合框", "选择框", "多行文本框", "搜索框", "密码输入框", "日期选择器", "时间选择器", "颜色选择器", "布尔属性控件", "数值属性行", "向量属性控件", "枚举属性控件", "引用属性控件"], documents.Select(x => x.ChineseName));
    }

    [Fact]
    public void DropDown_documentation_keeps_separator_free_semantic_boundary()
    {
        var split = XYUI2DocumentationCatalog.Build().Single(x => x.Id == "XYUI-2-04");
        var dropdown = XYUI2DocumentationCatalog.Build().Single(x => x.Id == "XYUI-2-05");
        Assert.NotEqual(split.CanonicalIdentity, dropdown.CanonicalIdentity);
        Assert.Equal("下拉按钮", dropdown.ChineseName);
        Assert.Equal("XYDropDownButton", dropdown.EnglishName);
    }

    [Fact]
    public void Gallery_preview_renders_dropdown_runtime_with_chevron_track() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-05");
        var window = XyuiBatchTestHost.Show(preview);
        var buttons = preview.GetVisualDescendants().OfType<XYDropDownButton>().ToList();
        Assert.True(buttons.Count >= 5, $"下拉按钮样例应覆盖五个状态，实测 {buttons.Count}");
        Assert.All(buttons.Where(x => x.IsEnabled), control => Assert.Contains(
            "xyui-icon", control.GetVisualDescendants().OfType<XYIcon>().Single().Classes));
        Assert.Equal(1, buttons.Count(x => !x.IsEnabled));
        window.Close();
    });
}
