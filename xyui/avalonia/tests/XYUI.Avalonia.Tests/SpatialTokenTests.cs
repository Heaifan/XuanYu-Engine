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
        Assert.Equal(XyuiSpatialTokens.Space1, Dim(MapValue("XY.Space.1")));
        Assert.Equal(XyuiSpatialTokens.Space2, Dim(MapValue("XY.Space.2")));
        Assert.Equal(XyuiSpatialTokens.Space3, Dim(MapValue("XY.Space.3")));
        Assert.Equal(XyuiSpatialTokens.Space4, Dim(MapValue("XY.Space.4")));
        Assert.Equal(XyuiSpatialTokens.Space6, Dim(MapValue("XY.Space.6")));
        Assert.Equal(XyuiSpatialTokens.Space8, Dim(MapValue("XY.Space.8")));
        Assert.Equal(XyuiSpatialTokens.Space10, Dim(MapValue("XY.Space.10")));
        Assert.Equal(XyuiSpatialTokens.Space12, Dim(MapValue("XY.Space.12")));
        Assert.Equal(XyuiSpatialTokens.PanelPadding, Dim(MapValue("XY.Panel.Padding")));
        Assert.Equal(XyuiSpatialTokens.FieldRowGap, Dim(MapValue("XY.Panel.Field.RowGap")));
    }

    [Fact]
    public void Radius_Matches_Canonical()
    {
        Assert.Equal(XyuiSpatialTokens.RadiusNone, Dim(MapValue("XY.Radius.None")));
        Assert.Equal(XyuiSpatialTokens.RadiusToolbar, Dim(MapValue("XY.Radius.Toolbar")));
        Assert.Equal(XyuiSpatialTokens.RadiusControl, Dim(MapValue("XY.Radius.Control")));
        Assert.Equal(XyuiSpatialTokens.RadiusInput, Dim(MapValue("XY.Radius.Input")));
        Assert.Equal(XyuiSpatialTokens.RadiusButton, Dim(MapValue("XY.Radius.Button")));
        Assert.Equal(XyuiSpatialTokens.RadiusPopup, Dim(MapValue("XY.Radius.Popup")));
        Assert.Equal(XyuiSpatialTokens.RadiusPanel, Dim(MapValue("XY.Radius.Panel")));
        Assert.Equal(XyuiSpatialTokens.RadiusRow, Dim(MapValue("XY.Radius.Row")));
        Assert.Equal(XyuiSpatialTokens.RadiusFull, Dim(MapValue("XY.Radius.Full")));
    }

    [Fact]
    public void Border_Width_Matches_Canonical()
    {
        Assert.Equal(XyuiSpatialTokens.BorderWidthNone, Dim(MapValue("XY.Border.Width.None")));
        Assert.Equal(XyuiSpatialTokens.BorderWidthDefault, Dim(MapValue("XY.Border.Width.Default")));
        Assert.Equal(XyuiSpatialTokens.BorderWidthStrong, Dim(MapValue("XY.Border.Width.Strong")));
        Assert.Equal(XyuiSpatialTokens.BorderWidthFocus, Dim(MapValue("XY.Border.Width.Focus")));
        Assert.Equal(XyuiSpatialTokens.BorderWidthSelected, Dim(MapValue("XY.Border.Width.Selected")));
    }

    [Fact]
    public void Shadow_Specs_Matches_Canonical()
    {
        Assert.Equal(XyuiSpatialTokens.ShadowTooltip, MapValue("XY.Shadow.Tooltip"));
        Assert.Equal(XyuiSpatialTokens.ShadowPopup, MapValue("XY.Shadow.Popup"));
        Assert.Equal(XyuiSpatialTokens.ShadowDragPreview, MapValue("XY.Shadow.DragPreview"));
        Assert.Equal(XyuiSpatialTokens.ShadowNone, "None");
    }
}
