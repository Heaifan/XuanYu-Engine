namespace XuanYu.World.Tests.UiTokens;

public sealed class MarkerActivationDomainContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));
    static string ReadEditor(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor", rel));

    [Fact]
    public void Marker_bootstrap_reports_point_domain_and_uses_unified_activation()
    {
        var bootstrap = Read("Vm/Map/UiVm.MapDataset.MarkerBootstrap.cs");
        var unified = Read("Vm/Map/UiVm.MapMarkerUnifiedDrawing.cs");
        var legacy = Read("Vm/Map/UiVm.MapMarkerPlacement.cs");

        Assert.Contains("CreateDatasetAsync(\"点标记绘制\")", bootstrap);
        Assert.DoesNotContain("BeginRegionDrawingAsync", bootstrap);
        Assert.Contains("MarkerDrawingController", ReadEditor("MapEditing/MarkerDrawingAdapter.cs"));
        Assert.Contains("DrawingCoordinator", ReadEditor("MapEditing/MarkerDrawingAdapter.cs"));
        Assert.Contains("DrawingPrimitiveKind.Point", ReadEditor("MapEditing/MarkerDrawingAdapter.cs"));
        Assert.Contains("\"MapMarker\"", ReadEditor("MapEditing/MarkerDrawingAdapter.cs"));
        Assert.Contains("return MarkerDrawingPointerReleased", legacy);
        Assert.DoesNotContain("CreateMarker", legacy);
        Assert.DoesNotContain("TryPickRegionPoint", unified);
    }
}
