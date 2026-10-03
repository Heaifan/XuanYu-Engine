using XuanYu.Editor.MapEditing;
using XuanYu.World;
using XuanYu.World.Map;
using XuanYu.World.Scene;

namespace XuanYu.Editor.Composition;

public static class EditorRuntimeComposition
{
    public static SceneStateOwner CreateScene(IWorldPartitionStrategy partitionStrategy,
        bool seedInitialEntity) => new(partitionStrategy, seedInitialEntity);

    public static MapEditSession CreateMapSession(Func<bool> isWriteThread) =>
        new(isWriteThread: isWriteThread);

    public static MapEditSession CreateEmptyWorldMapSession(Func<bool> isWriteThread) =>
        new(MapEmptyDefinition.Create(), isWriteThread);
}
