using System.Globalization;
using System.Text.Json;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

// Canonical 对照：Spatial/Shape token 常量必须与 token-canonical-map.json 逐条一致
public class SpatialTokenTests
{
    static readonly string MapPath = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
        "tokens", "architecture", "token-canonical-map.json");

    static string MapValue(string tokenId)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(MapPath));
        var e = doc.RootElement.GetProperty("entries").EnumerateArray()
            .First(x => x.GetProperty("canonical_token_id").GetString() == tokenId);
        return e.GetProperty("value").GetString()!;
    }

    static double Dim(string v) => double.Parse(v.Replace(" DIP", ""), CultureInfo.InvariantCulture);

    [Fact]
    public void Spacing_Matches_Canonical()
    {
        Assert.Equal(Dim(MapValue("XY.Space.1")), XyuiSpatialTokens.Space1);
        Assert.Equal(Dim(MapValue("XY.Space.2")), XyuiSpatialTokens.Space2);
        Assert.Equal(Dim(MapValue("XY.Space.3")), XyuiSpatialTokens.Space3);
        Assert.Equal(Dim(MapValue("XY.Space.4")), XyuiSpatialTokens.Space4);
        Assert.Equal(Dim(MapValue("XY.Space.6")), XyuiSpatialTokens.Space6);
        Assert.Equal(Dim(MapValue("XY.Space.8")), XyuiSpatialTokens.Space8);
        Assert.Equal(Dim(MapValue("XY.Space.10")), XyuiSpatialTokens.Space10);
        Assert.Equal(Dim(MapValue("XY.Space.12")), XyuiSpatialTokens.Space12);
        Assert.Equal(Dim(MapValue("XY.Panel.Padding")), XyuiSpatialTokens.PanelPadding);
        Assert.Equal(Dim(MapValue("XY.Panel.Field.RowGap")), XyuiSpatialTokens.FieldRowGap);
    }

    [Fact]
    public void Radius_Matches_Canonical()
    {
        Assert.Equal(Dim(MapValue("XY.Radius.None")), XyuiSpatialTokens.RadiusNone);
        Assert.Equal(Dim(MapValue("XY.Radius.Toolbar")), XyuiSpatialTokens.RadiusToolbar);
        Assert.Equal(Dim(MapValue("XY.Radius.Control")), XyuiSpatialTokens.RadiusControl);
        Assert.Equal(Dim(MapValue("XY.Radius.Input")), XyuiSpatialTokens.RadiusInput);
        Assert.Equal(Dim(MapValue("XY.Radius.Button")), XyuiSpatialTokens.RadiusButton);
        Assert.Equal(Dim(MapValue("XY.Radius.Popup")), XyuiSpatialTokens.RadiusPopup);
        Assert.Equal(Dim(MapValue("XY.Radius.Panel")), XyuiSpatialTokens.RadiusPanel);
        Assert.Equal(Dim(MapValue("XY.Radius.Row")), XyuiSpatialTokens.RadiusRow);
        Assert.Equal(Dim(MapValue("XY.Radius.Full")), XyuiSpatialTokens.RadiusFull);
    }

    [Fact]
    public void Border_Width_Matches_Canonical()
    {
        Assert.Equal(Dim(MapValue("XY.Border.Width.None")), XyuiSpatialTokens.BorderWidthNone);
        Assert.Equal(Dim(MapValue("XY.Border.Width.Default")), XyuiSpatialTokens.BorderWidthDefault);
        Assert.Equal(Dim(MapValue("XY.Border.Width.Strong")), XyuiSpatialTokens.BorderWidthStrong);
        Assert.Equal(Dim(MapValue("XY.Border.Width.Focus")), XyuiSpatialTokens.BorderWidthFocus);
        Assert.Equal(Dim(MapValue("XY.Border.Width.Selected")), XyuiSpatialTokens.BorderWidthSelected);
    }

    [Fact]
    public void Shadow_Specs_Matches_Canonical()
    {
        Assert.Equal(MapValue("XY.Shadow.Tooltip"), XyuiSpatialTokens.ShadowTooltip);
        Assert.Equal(MapValue("XY.Shadow.Popup"), XyuiSpatialTokens.ShadowPopup);
        Assert.Equal(MapValue("XY.Shadow.DragPreview"), XyuiSpatialTokens.ShadowDragPreview);
        Assert.Equal("None", XyuiSpatialTokens.ShadowNone);
    }
}
