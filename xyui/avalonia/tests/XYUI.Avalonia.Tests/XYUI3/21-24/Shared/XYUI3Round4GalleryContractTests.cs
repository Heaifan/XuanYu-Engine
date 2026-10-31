using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3Round4GalleryContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3Round4GalleryContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    static readonly string[] Ids = ["XYUI-3-3.19", "XYUI-3-3.20", "XYUI-3-3.21", "XYUI-3-3.22", "XYUI-3-3.23", "XYUI-3-3.24"];

    [Fact]
    public void Round4_documents_expose_real_quick_start_and_live_composition_factories() => _fx.Run(() =>
    {
        var documents = XYUI3DocumentationCatalog.Build();
        foreach (var id in Ids)
        {
            var document = Assert.Single(documents, x => x.Id == id);
            Assert.NotEmpty(document.QuickStartXaml);
            Assert.DoesNotContain("var ", document.QuickStartXaml, StringComparison.Ordinal);
            Assert.DoesNotContain("new ", document.QuickStartXaml, StringComparison.Ordinal);
            Assert.DoesNotContain("Build(", document.QuickStartXaml, StringComparison.Ordinal);
            Assert.NotNull(document.LiveExamplesFactory);
            Assert.NotNull(document.CompositionFactory);
            Assert.NotNull(XYUI3GalleryCatalog.CreatePreview(id));
        }
    });
}
