using System.IO;
using Xunit;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class FeatureEditInspectorContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        System.AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Inspector_edit_geometry_binding_contract()
    {
        var inspector = Read("Right/InspectorPanel.axaml");
        
        Assert.Contains("IsVisible=\"{Binding IsMapGeometrySelected}\"", inspector);
        Assert.Contains("IsChecked=\"{Binding IsGeometryEditingActive}\"", inspector);
        Assert.Contains("Command=\"{Binding ToggleGeometryEditingCommand}\"", inspector);
        Assert.Contains("编辑几何", inspector);
    }
}
