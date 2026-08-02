using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World;

namespace XuanYu.World.Scene;

public sealed partial class SceneStateOwner
{
    // D3：静态模型只是 World 的一种普通实体类型。World 不接收 AssetId、
    // RenderKey、GLB 路径或 GPU 资源；extent 是模型的局部空间描述，供
    // Picking / 空间查询使用，与立方体实体的空间描述同源。
    public WorldEntitySnapshot AddStaticModelEntity(
        string name,
        CommittedTransform transform,
        SpatialAabb extent)
    {
        var uniqueName = WorldEntityName.Unique(name, Entities);
        var entity = CreateEntity(uniqueName, WorldEntityTypes.StaticModel, transform, extent);
        SetActiveEntity(entity.EntityKey);
        return entity;
    }
}
