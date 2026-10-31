using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class test_LayerPresenter
{
    [Fact]
    public void Refresh_reprojects_rows_and_preserves_only_layer_identity()
    {
        var vm = new UiVm(null, () => true);
        var presenter = new LayerPresenter();
        MapLayerRowViewModel CreateRow(MapLayer layer) => new(vm, layer);

        presenter.Refresh(vm.MapSession.CurrentMap.Layers, vm.MapSession.ActiveRegionLayerId, CreateRow);
        var selected = presenter.Items[0];
        Assert.True(presenter.SetSelectedLayer(selected));

        presenter.Refresh(vm.MapSession.CurrentMap.Layers, vm.MapSession.ActiveRegionLayerId, CreateRow);

        Assert.NotSame(selected, presenter.SelectedLayer);
        Assert.Equal(selected.LayerId, presenter.SelectedLayer!.LayerId);
        Assert.All(presenter.Items, row => Assert.Equal(
            row.LayerId == vm.MapSession.ActiveRegionLayerId, row.IsActive));
    }

    [Fact]
    public void Clear_selection_removes_layer_display_selection()
    {
        var vm = new UiVm(null, () => true);
        var presenter = new LayerPresenter();
        presenter.Refresh(vm.MapSession.CurrentMap.Layers, vm.MapSession.ActiveRegionLayerId,
            layer => new MapLayerRowViewModel(vm, layer));
        presenter.SetSelectedLayer(presenter.Items[0]);

        Assert.True(presenter.ClearSelection());
        Assert.Null(presenter.SelectedLayer);
        Assert.False(presenter.HasSelection);
    }

    [Fact]
    public void UiVm_selection_keeps_existing_layer_binding_notifications()
    {
        var vm = new UiVm(null, () => true);
        var notifications = new List<string?>();
        vm.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);

        vm.SelectedLayer = vm.LayerItems[0];

        Assert.Contains(nameof(vm.HasLayerSelection), notifications);
        Assert.Contains(nameof(vm.CanMoveLayerUp), notifications);
        Assert.Contains(nameof(vm.CanDeleteLayer), notifications);
        Assert.Contains(nameof(vm.LayerInspectorNameText), notifications);
        Assert.Equal(vm.SelectedLayer!.Name, vm.LayerInspectorNameText);
    }
}
