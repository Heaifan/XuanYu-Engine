using Avalonia;
using Avalonia.Styling;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class GalleryThemeConstructionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public GalleryThemeConstructionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Current_foundation_and_xyui1_pages_construct_in_both_themes() => _fx.Run(() =>
    {
        var app = Application.Current!;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries());
        var documents = XYUI1DocumentationCatalog.Build();
        Assert.Equal(24, documents.Count);
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        {
            app.RequestedThemeVariant = variant;
            Assert.NotNull(new PaletteView());
            Assert.NotNull(new TypographyView());
            Assert.NotNull(new ShapeView());
            foreach (var document in documents) Assert.NotNull(document.PreviewFactory());
        }
        app.RequestedThemeVariant = ThemeVariant.Light;
    });
}
