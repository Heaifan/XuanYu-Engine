using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.StaticModels;

public sealed class RegionModelTransformContractTests
{
    [Fact]
    public void R18_world_space_region_model_uses_identity_transform()
    {
        var transform = RenderStaticModelTransform.Identity;

        Assert.Equal(Vector3d.Zero, transform.Position);
        Assert.Equal(Vector3d.Zero, transform.Rotation);
        Assert.Equal(new Vector3d(1, 1, 1), transform.Scale);
    }
}
