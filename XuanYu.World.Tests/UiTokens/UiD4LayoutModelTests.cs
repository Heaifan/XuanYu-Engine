using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4：纯布局逻辑测试——检查器/地图模式阈值、MapId 显示压缩、字段行结构。
public sealed class UiD4LayoutModelTests
{
    [Fact]
    public void Inspector_mode_switches_at_360_content_width()
    {
        Assert.Equal(InspectorFormMode.Narrow, InspectorLayoutModel.ModeFor(0));
        Assert.Equal(InspectorFormMode.Narrow, InspectorLayoutModel.ModeFor(359));
        Assert.Equal(InspectorFormMode.Wide, InspectorLayoutModel.ModeFor(360));
        Assert.Equal(InspectorFormMode.Wide, InspectorLayoutModel.ModeFor(600));
    }

    [Fact]
    public void Inspector_columns_match_spec_contract()
    {
        Assert.Equal(360, InspectorLayoutModel.WideThreshold);       // §7.1 表单纵向切换阈值
        Assert.Equal(96, InspectorLayoutModel.LabelColumnWidth);     // §5.3 标签列默认
        Assert.Equal(128, InspectorLayoutModel.FieldMinWidth);       // §5.3 字段最小
    }

    [Fact]
    public void Map_editor_mode_switches_at_320_content_width()
    {
        Assert.Equal(MapEditorDensityMode.Compact, MapEditorLayoutModel.ModeFor(319));
        Assert.Equal(MapEditorDensityMode.Standard, MapEditorLayoutModel.ModeFor(320));
    }

    [Fact]
    public void Map_id_short_value_stays_unchanged()
    {
        Assert.Equal("abc", MapIdDisplayFormat.Format("abc"));
        Assert.Equal("123456789012345678", MapIdDisplayFormat.Format("123456789012345678")); // 恰 18 不压缩
    }

    [Fact]
    public void Map_id_long_value_shows_head_ellipsis_tail()
    {
        const string longId = "a1ea150c-3f2b-4c6d-9e8a-d9ea5c7b2f11";
        Assert.Equal("a1ea150c…7b2f11", MapIdDisplayFormat.Format(longId));
    }

    [Fact]
    public void Map_id_format_constants_match_contract()
    {
        Assert.Equal(18, MapIdDisplayFormat.MaxPlainLength);
        Assert.Equal(8, MapIdDisplayFormat.HeadLength);
        Assert.Equal(6, MapIdDisplayFormat.TailLength);
    }

    [Fact]
    public void Inspector_field_row_supports_group_headers()
    {
        var group = new InspectorFieldRow("变换", "", IsGroupHeader: true);
        var field = new InspectorFieldRow("名称", "立方体");
        Assert.True(group.IsGroupHeader);
        Assert.False(field.IsGroupHeader);
        Assert.Equal("立方体", field.Value);
    }
}
