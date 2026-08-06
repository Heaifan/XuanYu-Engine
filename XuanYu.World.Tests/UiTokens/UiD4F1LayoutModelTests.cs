using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4-F1（纠偏 v2）：双模型并存且互不替代——
//  MapEditorLayoutModel（<320 面板紧凑密度）与 EditableFormLayoutModel（<360 输入表单方向）。
public sealed class UiD4F1LayoutModelTests
{
    [Fact]
    public void Map_editor_density_switches_at_320()
    {
        Assert.Equal(MapEditorDensityMode.Compact, MapEditorLayoutModel.ModeFor(0));
        Assert.Equal(MapEditorDensityMode.Compact, MapEditorLayoutModel.ModeFor(319));
        Assert.Equal(MapEditorDensityMode.Standard, MapEditorLayoutModel.ModeFor(320));
        Assert.Equal(MapEditorDensityMode.Standard, MapEditorLayoutModel.ModeFor(480));
    }

    [Fact]
    public void Editable_form_switches_at_360()
    {
        Assert.Equal(EditableFormMode.Narrow, EditableFormLayoutModel.ModeFor(359));
        Assert.Equal(EditableFormMode.Wide, EditableFormLayoutModel.ModeFor(360));
    }

    [Theory]
    [InlineData(300, MapEditorDensityMode.Compact, EditableFormMode.Narrow)]
    [InlineData(319, MapEditorDensityMode.Compact, EditableFormMode.Narrow)]
    [InlineData(320, MapEditorDensityMode.Standard, EditableFormMode.Narrow)] // 密度恢复标准，表单仍窄（320<360）
    [InlineData(359, MapEditorDensityMode.Standard, EditableFormMode.Narrow)]
    [InlineData(360, MapEditorDensityMode.Standard, EditableFormMode.Wide)]
    [InlineData(480, MapEditorDensityMode.Standard, EditableFormMode.Wide)]
    public void Density_and_form_models_are_independent(
        double width, MapEditorDensityMode density, EditableFormMode form)
    {
        // 纠偏 v2：两种模式各自按阈值判定，互不替代
        Assert.Equal(density, MapEditorLayoutModel.ModeFor(width));
        Assert.Equal(form, EditableFormLayoutModel.ModeFor(width));
    }

    static string? _panel;
    static string ReadPanel() =>
        _panel ??= System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    [Theory]
    [InlineData(300)]
    [InlineData(320)]
    [InlineData(340)]
    [InlineData(360)]
    [InlineData(480)]
    public void Readonly_key_value_rows_never_switch_to_vertical(double width)
    {
        // 只读键值行无布局模式概念：检查器无双布局树，任何宽度保持单行双列。
        _ = width;
        Assert.DoesNotContain("WideFields", ReadPanel());
        Assert.DoesNotContain("NarrowFields", ReadPanel());
    }
}
