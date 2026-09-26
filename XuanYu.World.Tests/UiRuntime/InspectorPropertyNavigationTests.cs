using XuanYu.Editor.UI;
using Xunit;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class InspectorPropertyNavigationTests
{
    [Fact]
    public void Recent_is_scoped_by_object_kind_and_keeps_six_keys()
    {
        var recent = new InspectorRecentStore();
        for (var i = 1; i <= 7; i++) recent.Record(InspectorObjectKind.Road, $"Road.Property{i}");
        recent.Record(InspectorObjectKind.Road, "Road.Property3");
        recent.Record(InspectorObjectKind.Region, "Region.Property1");

        Assert.Equal(new[] { "Road.Property3", "Road.Property7", "Road.Property6", "Road.Property5", "Road.Property4", "Road.Property2" },
            recent.KeysFor(InspectorObjectKind.Road));
        Assert.Equal(new[] { "Region.Property1" }, recent.KeysFor(InspectorObjectKind.Region));
    }

    [Fact]
    public void Search_matches_display_name_across_categories_and_alias_case_insensitively()
    {
        var properties = new[]
        {
            new InspectorPropertyDescriptor("Road.Basic.Name", InspectorObjectKind.Road, "基础", "名称", "道路名称"),
            new InspectorPropertyDescriptor("Road.Geometry.Width", InspectorObjectKind.Road, "几何", "尺寸", "道路宽度"),
            new InspectorPropertyDescriptor("Road.Status.Access", InspectorObjectKind.Road, "状态", "通行", "通行状态", "access")
        };

        Assert.Equal("Road.Geometry.Width", Assert.Single(InspectorPropertySearch.Find(properties, "宽")).Key);
        Assert.Equal("Road.Status.Access", Assert.Single(InspectorPropertySearch.Find(properties, "ACCESS")).Key);
    }

    [Fact]
    public void Failed_commit_does_not_enter_recent()
    {
        var recent = new InspectorRecentStore();

        recent.RecordCommit(InspectorObjectKind.Road, "Road.Geometry.Width", succeeded: false);

        Assert.Empty(recent.KeysFor(InspectorObjectKind.Road));
    }

    [Fact]
    public void Selection_drives_kind_categories_without_using_mode()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.First(item => item.IsEntity);

        Assert.Equal(InspectorObjectKind.Entity, vm.InspectorIdentity);
        Assert.Contains("基础", vm.InspectorCategories);
        Assert.Contains("几何", vm.InspectorCategories);
    }

    [Fact]
    public void Property_context_path_is_hidden_on_categories_and_visible_in_search_and_recent()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var dataset = new MapDatasetRow("区域数据", "region", "region-dataset", "正常", "data/region.json");
        typeof(UiVm).GetField("_datasetItems", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(vm, new[] { dataset });
        vm.DatasetSelectedId = dataset.Id;

        Assert.All(vm.InspectorProperties, row => Assert.False(row.ShowContextPath));
        vm.InspectorSearchText = "名称";
        Assert.NotEmpty(vm.InspectorProperties);
        Assert.All(vm.InspectorProperties, row => Assert.True(row.ShowContextPath));
        vm.InspectorSearchText = "";
        vm.RecordInspectorCommit(new(InspectorObjectKind.Dataset, dataset.Id, "Dataset.Basic.Name"), true);
        vm.SelectInspectorCategoryCommand.Execute("最近");
        Assert.All(vm.InspectorProperties, row => Assert.True(row.ShowContextPath));
    }
}
