using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldScaleTransformUiTests
{
    static UiVm ScaleVm()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("缩放");
        return vm;
    }

    static UiVm ScaleVmTwoEntities(out SceneStateOwner scene, out EntityId bKey)
    {
        var vm = new UiVm(null, () => true);
        var field = typeof(UiVm).GetField("_sceneState",
            BindingFlags.Instance | BindingFlags.NonPublic);
        scene = (SceneStateOwner)field!.GetValue(vm)!;
        bKey = scene.CreateEntity("实体B", "Unit",
            new CommittedTransform(new Vector3d(5, 0, 0), Vector3d.Zero,
                new Vector3d(1, 1, 1))).EntityKey;
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("缩放");
        vm.UpdateViewportFrame(800, 600);
        return vm;
    }

    static EditorHistoryOwner HistoryOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField(
            "_historyOwner",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (EditorHistoryOwner)field!.GetValue(vm)!;
    }
}
