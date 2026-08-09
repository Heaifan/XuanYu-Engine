using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// MAP-A-R2-D5-F3：图层行视觉层级与拖拽入口合同。
public sealed class UiF3LayerRowContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerPanel.axaml"));

    [Fact]
    public void Layer_name_is_expanding_body_between_handle_and_states()
    {
        Assert.Contains("ColumnDefinitions=\"3,24,Auto,*,28,28\"", Panel);
        Assert.Contains("Grid.Column=\"1\" Classes=\"dragHandle\"", Panel);
        Assert.Contains("Grid.Column=\"3\" Classes=\"layerName\"", Panel);
        Assert.Contains("Grid.Column=\"4\" Classes=\"layerSwitch\"", Panel);
        Assert.Contains("Grid.Column=\"5\" Classes=\"layerLockSwitch\"", Panel);
        Assert.Contains("HorizontalContentAlignment\" Value=\"Stretch\"", Panel);
    }

    [Fact]
    public void Reorder_hint_is_secondary_help_text()
    {
        Assert.Contains("Classes=\"layerHint\"", Panel);
        Assert.Contains("添加至少 2 个用户图层后可拖拽排序", Panel);
    }

    [Fact]
    public void Drag_events_are_bound_only_to_the_handle()
    {
        const string binding = "PointerPressed=\"DragHandle_PointerPressed\"";
        Assert.Equal(1, Panel.Split(binding, StringSplitOptions.None).Length - 1);
        Assert.Contains("Style Selector=\"Border.dragHandle\"", Panel);
        Assert.Contains("Style Selector=\"Path.dragHandleIcon\"", Panel);
    }
}
