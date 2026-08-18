using Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI1DocumentationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public XYUI1DocumentationTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Documentation_has_module_overview_and_24_component_pages() => _fx.Run(() =>
    {
        var model = new XYUI1DocumentationViewModel();
        Assert.Equal(25, model.Items.Count);
        Assert.Equal(24, model.Documents.Count);
        Assert.Null(model.Items[0].Document);
        Assert.All(model.Documents, document =>
        {
            Assert.IsAssignableFrom<Control>(document.PreviewFactory());
            Assert.NotSame(document.PreviewFactory(), document.PreviewFactory());
            Assert.NotEmpty(document.ChineseName);
            Assert.NotEmpty(document.Overview);
            Assert.NotEmpty(document.Usages);
            Assert.All(document.Properties, property => Assert.DoesNotContain("None defined", property.Description));
        });
    });

    [Fact]
    public void Navigation_selects_a_real_component_document() => _fx.Run(() =>
    {
        var model = new XYUI1DocumentationViewModel();
        model.Select("XYUI-1-04");
        Assert.Equal("标题", model.SelectedItem.ChineseName);
        Assert.IsType<XYUI1ComponentDocumentView>(model.SelectedDocument);
        Assert.Equal("XYHeading", model.SelectedItem.Document!.AvaloniaType);
    });

    [Fact]
    public void Foundation_navigation_selects_existing_foundation_views() => _fx.Run(() =>
    {
        var model = new XYUI1DocumentationViewModel();
        model.SelectFoundation("palette");
        Assert.IsType<PaletteView>(model.SelectedDocument);
        model.SelectFoundation("typography");
        Assert.IsType<TypographyView>(model.SelectedDocument);
        model.SelectFoundation("shape");
        Assert.IsType<ShapeView>(model.SelectedDocument);
    });
}
