using System;
using XuanYu.Editor.Composition;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan;
using XuanYu.World.Scene;
using XuanYu.World;

namespace XuanYu.Editor.App;

public static class EditorCompositionRoot
{
    public static INativeHostSurfaceBridgeFactory CreateSurfaceBridgeFactory() =>
        new VulkanNativeHostSurfaceBridgeFactory();

    public static (SceneStateOwner Scene, MapEditSession Map) CreateEditorState(
        Func<bool> isWriteThread, bool seedInitialScene) =>
        (EditorRuntimeComposition.CreateScene(
                new GridWorldPartitionStrategy(regionSize: 5), seedInitialScene),
            EditorRuntimeComposition.CreateEmptyWorldMapSession(isWriteThread));
}
