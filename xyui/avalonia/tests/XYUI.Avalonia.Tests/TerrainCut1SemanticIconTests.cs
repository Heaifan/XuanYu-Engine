using System.Reflection;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

public sealed class TerrainCut1SemanticIconTests
{
    [Fact]
    public void RegionPolygon_and_TerrainRelief_are_distinct_registered_geometries()
    {
        Assert.True(Enum.IsDefined(typeof(XyuiVectorIcon), "RegionPolygon"));
        Assert.True(Enum.IsDefined(typeof(XyuiVectorIcon), "TerrainRelief"));
        var region = XyuiVectorIcons.PathData[Enum.Parse<XyuiVectorIcon>("RegionPolygon")];
        var terrain = XyuiVectorIcons.PathData[Enum.Parse<XyuiVectorIcon>("TerrainRelief")];
        Assert.NotEqual(region, terrain);
        Assert.NotEqual(XyuiVectorIcons.PathData[XyuiVectorIcon.BoxSelect], region);
    }

    [Theory]
    [InlineData("area", "RegionPolygon")]
    [InlineData("terrain", "TerrainRelief")]
    public void Context_category_uses_the_semantic_icon(string categoryId, string expectedIcon)
    {
        Assert.Equal(expectedIcon, InvokeIcon("CategoryIcon", categoryId)?.ToString());
    }

    [Fact]
    public void Region_action_uses_the_region_polygon_icon()
    {
        Assert.Equal("RegionPolygon", InvokeIcon("ActionIcon", "region")?.ToString());
    }

    static XyuiVectorIcon? InvokeIcon(string methodName, string id)
    {
        var method = typeof(XYContextDropdownBoard).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)!;
        var result = method.Invoke(null, [id]);
        return (XyuiVectorIcon?)result;
    }
}
