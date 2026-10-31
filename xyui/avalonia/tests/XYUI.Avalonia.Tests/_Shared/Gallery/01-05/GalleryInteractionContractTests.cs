using System.Runtime.CompilerServices;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public class GalleryInteractionContractTests
{
    [Fact]
    public void Interaction_View_Uses_One_Scroll_Owner_And_Spatial_Tokens()
    {
        var text = File.ReadAllText(ViewPath());
        Assert.DoesNotContain("<ScrollViewer>", text);
        Assert.DoesNotContain("Spacing=\"16\"", text);
        Assert.DoesNotContain("Spacing=\"8\"", text);
        Assert.DoesNotContain("Spacing=\"6\"", text);
        Assert.DoesNotContain("Padding=\"12\"", text);
        Assert.DoesNotContain("Margin=\"8\"", text);
        Assert.DoesNotContain("Width=\"240\"", text);
        Assert.DoesNotContain("图层 A        可见", text);
        Assert.Contains("XY.Panel.Padding", text);
        Assert.Contains("XY.Panel.SectionGap", text);
        Assert.Contains("ColumnDefinitions=\"*,Auto\"", text);
    }

    static string ViewPath([CallerFilePath] string source = "")
    {
        var dir = new DirectoryInfo(Path.GetDirectoryName(source)!);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "AGENTS.md"))) dir = dir.Parent;
        var root = Path.Combine(dir?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
            "xyui", "avalonia", "gallery", "XYUI.Avalonia.Gallery", "Views");
        return Directory.GetFiles(root, "InteractionStatesView.axaml", SearchOption.AllDirectories).Single();
    }
}
