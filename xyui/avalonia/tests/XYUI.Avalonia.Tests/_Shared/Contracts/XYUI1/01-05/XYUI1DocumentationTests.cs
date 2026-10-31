using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
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
        Assert.Equal(24, model.VisualAcceptedCount);
        Assert.All(model.Documents, document => Assert.StartsWith("BASELINE ACCEPTED · MIGRATION REVIEW", document.StatusText));
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
    public void XYUI2_documentation_has_category_and_separated_radio_groups() => _fx.Run(() =>
    {
        var model = new XYUI1DocumentationViewModel();
        Assert.All(model.XYUI2Items.Skip(1), item => Assert.Equal("Canonical Stable · Buttons / Inputs", item.Document!.Category));
        Assert.All(model.XYUI2Items.Skip(1).SelectMany(x => x.Document!.HowToUse), guide => Assert.NotEqual("", guide.Category));
        var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-07");
        var groups = preview.GetVisualDescendants().OfType<XYRadioButton>().GroupBy(x => x.GroupName).ToArray();
        Assert.Equal(2, groups.Length); Assert.Equal(new[] { 3, 2 }, groups.Select(x => x.Count()).OrderByDescending(x => x));
        Assert.All(Enumerable.Range(13, 12).Select(i => $"XYUI-2-{i:D2}"), id => Assert.NotNull(XYUI2LiveExamplesFactory.Create(id)));
    });

    [Fact]
    public void Phase2C_quick_start_preview_count_is_at_most_two() => _fx.Run(() =>
    {
        var ids = new[] { "XYUI-2-13", "XYUI-2-14", "XYUI-2-15", "XYUI-2-16", "XYUI-2-17", "XYUI-2-18" };
        Assert.All(ids, id =>
        {
            var document = XYUI2DocumentationCatalog.Build().Single(x => x.Id == id);
            var preview = document.PreviewFactory();
            Assert.InRange(preview.GetVisualDescendants().Count(x => x is XYSelect or XYTextArea or XYSearchField or XYPasswordField or XYDatePicker or XYTimePicker), 1, 2);
        });
    });

    [Fact]
    public void Foundation_navigation_selects_existing_foundation_views() => _fx.Run(() =>
    {
        var model = new XYUI1DocumentationViewModel();
        AssertFoundation<PaletteView>(model, "palette");
        AssertFoundation<TypographyView>(model, "typography");
        AssertFoundation<SpacingLayoutView>(model, "spacing_layout");
        AssertFoundation<SizingView>(model, "sizing");
        AssertFoundation<DensitySamplesView>(model, "density");
        AssertFoundation<IconographyView>(model, "iconography");
        AssertFoundation<ShapeView>(model, "shape");
        AssertFoundation<SurfaceView>(model, "surface");
        AssertFoundation<StatesView>(model, "states");
        AssertFoundation<ResponsiveView>(model, "responsive");
        AssertFoundation<AccessibilityView>(model, "accessibility");
        AssertFoundation<LayoutRecipesView>(model, "layout_recipes");
    });
    static void AssertFoundation<T>(XYUI1DocumentationViewModel model, string key) { model.SelectFoundation(key); Assert.IsType<T>(model.SelectedDocument); }
}
