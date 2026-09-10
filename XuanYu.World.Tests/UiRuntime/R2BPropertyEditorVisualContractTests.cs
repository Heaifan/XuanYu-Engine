using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System.IO;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class R2BPropertyEditorVisualContractTests
{
    readonly UiHeadlessFixture _fixture;

    public R2BPropertyEditorVisualContractTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Map_form_declares_compact_property_geometry()
    {
        var form = Read("XuanYu.Editor.UI", "Right", "MapFormPanel.axaml");
        Assert.Equal(3, Count(form, "<xy:XYNumberField"));
        Assert.DoesNotContain("<xy:XYTextField", form);
        Assert.Equal(3, Count(form, "Width=\"{StaticResource Size.Width.128}\""));
        Assert.Equal(6, Count(form, "Height=\"{StaticResource Control.Height.Compact}\""));
        Assert.Equal(4, Count(form, "xycore:XY.Size=\"Compact\""));
        Assert.Equal(3, Count(form, "Width=\"{StaticResource Size.Width.96}\""));
        Assert.DoesNotContain("HorizontalAlignment=\"Stretch\"", form);
        Assert.DoesNotContain("PropsNarrow", form);
        Assert.DoesNotContain("EditableFormLayoutModel", form);
        Assert.Contains("ColumnDefinitions=\"96,*\"", form);
        Assert.Equal(9, Count(form, "VerticalAlignment=\"Center\""));
        Assert.Contains("RowSpacing=\"{StaticResource Space.4}\"", form);
        Assert.Contains("PropsButtons", form);
        Assert.Contains("Padding=\"{StaticResource Padding.Compact}\"", form);
    }

    [Fact]
    public void Map_form_materializes_compact_inputs_and_actions()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var form = new MapFormPanel { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(form, 720, 640); form.UpdateLayout();
            var fields = UiRuntimeTestHost.Descendants<XYNumberField>(form)
                .Select(field => (field.Width, field.Height, field.HorizontalAlignment)).ToArray();
            var buttons = UiRuntimeTestHost.Descendants<XYButton>(form)
                .Select(button => (button.Width, button.Height, button.HorizontalAlignment)).ToArray();
            return (Fields: fields, Buttons: buttons);
        });

        Assert.Equal(3, result.Fields.Length);
        Assert.All(result.Fields, field =>
        {
            Assert.Equal(128, field.Width);
            Assert.Equal(24, field.Height);
            Assert.Equal(HorizontalAlignment.Left, field.HorizontalAlignment);
        });
        Assert.Equal(3, result.Buttons.Length);
        Assert.All(result.Buttons, button =>
        {
            Assert.Equal(96, button.Width);
            Assert.Equal(24, button.Height);
            Assert.Equal(HorizontalAlignment.Left, button.HorizontalAlignment);
        });
    }

    [Fact]
    public void Map_form_business_wiring_is_preserved()
    {
        var form = Read("XuanYu.Editor.UI", "Right", "MapFormPanel.axaml");
        foreach (var binding in new[] { "MapWidthDraft", "MapDepthDraft", "MapBaseHeightDraft" })
            Assert.Contains(binding, form);
        Assert.Equal(3, Count(form, "LostFocus=\"Field_LostFocus\""));
        foreach (var command in new[] { "应用地图属性", "撤销地图修改", "重做地图修改" })
            Assert.Contains($"CommandParameter=\"{command}\"", form);
    }

    [Fact]
    public void Left_panel_stays_within_compact_width_contract()
    {
        var shell = Read("XuanYu.Editor.UI", "Root", "UiRoot.axaml");
        Assert.Contains("Width=\"216\" MinWidth=\"210\" MaxWidth=\"220\"", shell);
        Assert.DoesNotContain("Width=\"270\"", shell);
        Assert.DoesNotContain("MaxWidth=\"420\"", shell);
    }

    static string Read(params string[] path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(path)));

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

