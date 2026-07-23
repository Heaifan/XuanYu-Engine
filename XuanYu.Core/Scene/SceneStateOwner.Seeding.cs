using XuanYu.Core.Math;

namespace XuanYu.Core.Scene;

public sealed partial class SceneStateOwner
{
    public void EnsureEntityCount(int count)
    {
        if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));
        while (Entities.Count < count)
        {
            var index = Entities.Count + 1;
            var x = (index - 1) % 5;
            var y = (index - 1) / 5;
            CreateEntity(
                $"测试实体 {index:00}",
                "MinimalSceneEntity",
                new CommittedTransform(new Vector3d(x * 1.5, y * 1.5, 0)));
        }
    }
}
