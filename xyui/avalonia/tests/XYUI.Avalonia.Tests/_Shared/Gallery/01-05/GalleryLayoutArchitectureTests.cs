using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed partial class GalleryLayoutArchitectureTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public GalleryLayoutArchitectureTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void MigratedDocuments_UseSharedSemanticRows()
    {
        var legacy = ReadView("XYUI1ComponentDocumentView.axaml");
        var modern = ReadView("XYUI3ComponentDocumentView.axaml");
        var core = ReadView("XYUI3CoreRulesSection.axaml");
        Assert.Contains("GalleryDocumentShell", legacy);
        Assert.Contains("GalleryDocumentShell", modern);
        Assert.Contains("GalleryRuleRow", legacy); Assert.Contains("GalleryVariantRow", legacy);
        Assert.Contains("GalleryTokenRow", legacy); Assert.Contains("GalleryStateRow", legacy);
        Assert.Contains("GalleryRuleRow", core); Assert.Contains("GalleryTokenRow", modern);
        Assert.DoesNotContain("ColumnDefinitions=\"120,*\"", legacy);
        Assert.DoesNotContain("ColumnDefinitions=\"120,*,*\"", legacy);
    }

    [Fact] public void RowImplementations_HaveOneSharedOwner()
    {
        var shared = ReadView("GalleryKeyValueRow.cs");
        var core = ReadView("XYUI3CoreRulesSection.axaml.cs");
        Assert.Contains("class GalleryRuleRow", shared);
        Assert.Contains("class GalleryStateRow", shared);
        Assert.Contains("class GalleryVariantRow", ReadView("GalleryVariantRow.cs"));
        Assert.Contains("class GalleryTokenRow", ReadView("GalleryTokenRow.cs"));
        Assert.DoesNotContain("class XYUI1RuleRow", shared);
        Assert.DoesNotContain("class XYUI2RuleRow", shared);
        Assert.Contains(": GalleryRuleRow", core);
    }

    [Fact] public void SharedRuleRow_WrapsWithoutOverlap() => _fx.Run(() =>
    {
        var row = new GalleryRuleRow();
        row.Children.Add(new TextBlock { Text = "高密尺寸与左右双栏宽度严格控制" });
        row.Children.Add(new TextBlock { Text = "说明文本必须保留独立剩余宽度并允许长内容换行。" });
        row.Measure(new Size(640, double.PositiveInfinity));
        row.Arrange(new Rect(0, 0, 640, row.DesiredSize.Height));
        Assert.True(row.Children[0].Bounds.Right + 16 <= row.Children[1].Bounds.Left + 0.1);
        row.Measure(new Size(400, double.PositiveInfinity));
        row.Arrange(new Rect(0, 0, 400, row.DesiredSize.Height));
        Assert.True(row.Children[1].Bounds.Top >= row.Children[0].Bounds.Bottom + 5);
    });

    [Fact] public void RepresentativeCatalogRoutes_RemainPresent() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel();
        Assert.NotNull(vm.Items.Single(x => x.Id == "XYUI-1-19").Document);
        Assert.NotNull(vm.XYUI2Items.Single(x => x.Id == "XYUI-2-13").Document);
        Assert.NotNull(vm.XYUI3Items.Single(x => x.Id == "XYUI-3-3.18").Document);
    });

    static string Read(string relative)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "AGENTS.md")))
            dir = dir.Parent!;
        var path = Path.Combine(dir!.FullName, relative.Replace('/', Path.DirectorySeparatorChar));
        return File.ReadAllText(path);
    }

    static string ReadView(string name)
    {
        var root = Path.Combine(FindRepositoryRoot(), "xyui", "avalonia", "gallery",
            "XYUI.Avalonia.Gallery", "Views");
        var path = Directory.GetFiles(root, name, SearchOption.AllDirectories).SingleOrDefault();
        return File.ReadAllText(path ?? throw new FileNotFoundException($"Gallery view not found: {name}"));
    }

    static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "AGENTS.md"))) dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
