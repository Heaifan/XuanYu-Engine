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
        Assert.Equal(5, documents.Count);
        Assert.All(documents, document => Assert.StartsWith(
            XYUI2DocumentationCatalog.PendingAcceptance, document.StatusText));
        var dropdown = documents.Single(x => x.Id == "XYUI-2-05");
        Assert.Equal("XY.DropDownButton · XYDropDownButton", dropdown.CanonicalDisplay);
        Assert.All(dropdown.Usages, usage => Assert.Contains("XYDropDownButton", usage));
        Assert.Contains(dropdown.Tokens, token => token.Name == "XY.DropDownButton.Height");
    }

    [Fact]
    public void DropDown_documentation_keeps_separator_free_semantic_boundary()
    {
        var split = XYUI2DocumentationCatalog.Build().Single(x => x.Id == "XYUI-2-04");
        var dropdown = XYUI2DocumentationCatalog.Build().Single(x => x.Id == "XYUI-2-05");
        Assert.NotEqual(split.CanonicalIdentity, dropdown.CanonicalIdentity);
        Assert.Equal("DropDown Button", dropdown.ChineseName);
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
