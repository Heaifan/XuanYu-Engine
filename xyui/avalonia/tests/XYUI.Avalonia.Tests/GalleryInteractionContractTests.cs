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

    static string ViewPath([CallerFilePath] string source = "") => Path.GetFullPath(
        Path.Combine(Path.GetDirectoryName(source)!, "..", "..", "gallery",
            "XYUI.Avalonia.Gallery", "Views", "InteractionStatesView.axaml"));
}
