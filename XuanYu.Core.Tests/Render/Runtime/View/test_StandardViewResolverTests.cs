using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// F3-D3：六方向标准视角解析测试（计划 11.4——Pivot/距离保留、Up 合同、无滚转/镜像）。
public sealed class StandardViewResolverTests
{
    [Theory]
    [InlineData("+X 视图", -1.0, 0.0, 0.0)]
    [InlineData("-X 视图", 1.0, 0.0, 0.0)]
    [InlineData("+Y 视图", 0.0, -1.0, 0.0)]
    [InlineData("-Y 视图", 0.0, 1.0, 0.0)]
    [InlineData("顶视图", 0.0, 0.0, -1.0)]
    [InlineData("底视图", 0.0, 0.0, 1.0)]
    public void Six_directions_resolve_forward(string name, double fx, double fy, double fz)
    {
        Assert.True(StandardViewResolver.TryResolve(name, out var forward, out _), $"{name} 应可解析");
        Assert.Equal(fx, forward.X, 9);
        Assert.Equal(fy, forward.Y, 9);
        Assert.Equal(fz, forward.Z, 9);
    }

    // Up 合同：±X/±Y 侧视图 Up=+Z；顶视图 Up=+Y；底视图 Up=-Y（防镜像，保证 Right=+X）。
    [Fact]
    public void Up_contract_prevents_roll()
    {
        Assert.True(StandardViewResolver.TryResolve("+X 视图", out _, out var upX));
        Assert.Equal(0.0, upX.X, 9);
        Assert.Equal(0.0, upX.Y, 9);
        Assert.Equal(1.0, upX.Z, 9);
        Assert.True(StandardViewResolver.TryResolve("顶视图", out _, out var upTop));
        Assert.Equal(0.0, upTop.X, 9);
        Assert.Equal(1.0, upTop.Y, 9);
        Assert.Equal(0.0, upTop.Z, 9);
        Assert.True(StandardViewResolver.TryResolve("底视图", out _, out var upBottom));
        Assert.Equal(0.0, upBottom.X, 9);
        Assert.Equal(-1.0, upBottom.Y, 9); // F3-F2 合同修正：底视 -Y，Right 保持 +X
        Assert.Equal(0.0, upBottom.Z, 9);
    }

    // 底视图与顶视图互为反向且无翻转（同一 Up，方向相反）。
    [Fact]
    public void Top_bottom_are_opposites_without_mirror()
    {
        Assert.True(StandardViewResolver.TryResolve("顶视图", out var top, out _));
        Assert.True(StandardViewResolver.TryResolve("底视图", out var bottom, out _));
        Assert.Equal(-top.X, bottom.X, 9);
        Assert.Equal(-top.Y, bottom.Y, 9);
        Assert.Equal(-top.Z, bottom.Z, 9);
    }

    // Gizmo 端点名映射。
    [Theory]
    [InlineData("+X", "+X 视图")]
    [InlineData("-X", "-X 视图")]
    [InlineData("+Y", "+Y 视图")]
    [InlineData("-Y", "-Y 视图")]
    [InlineData("+Z", "顶视图")]
    [InlineData("-Z", "底视图")]
    public void Endpoint_to_view_name_mapping(string endpoint, string expected)
    {
        Assert.Equal(expected, StandardViewResolver.EndpointToViewName(endpoint));
    }

    // F3-F4：标准视图 → 视图平面网格映射（±X→YZ、±Y→XZ、±Z/其他→None）。
    [Theory]
    [InlineData("+X 视图", EditorViewPlaneGridKind.YZ)]
    [InlineData("-X 视图", EditorViewPlaneGridKind.YZ)]
    [InlineData("+Y 视图", EditorViewPlaneGridKind.XZ)]
    [InlineData("-Y 视图", EditorViewPlaneGridKind.XZ)]
    [InlineData("顶视图", EditorViewPlaneGridKind.None)]
    [InlineData("底视图", EditorViewPlaneGridKind.None)]
    [InlineData("默认视角", EditorViewPlaneGridKind.None)]
    public void View_plane_grid_kind_maps_standard_views(string name, EditorViewPlaneGridKind kind)
    {
        Assert.Equal(kind, StandardViewResolver.ViewPlaneGridFor(name));
    }

    // 相机位置：Pivot 不变、距离不变（Position = center - forward × distance）。
    [Fact]
    public void Camera_position_preserves_pivot_and_distance()
    {
        var center = new Vector3d(10, 20, 30);
        const double distance = 50.0;
        Assert.True(StandardViewResolver.TryResolve("+X 视图", out var forward, out _));
        var position = center - (forward * distance);
        Assert.Equal(center.X + distance, position.X, 9); // 相机在 +X 侧
        Assert.Equal(center.Y, position.Y, 9);
        Assert.Equal(center.Z, position.Z, 9);
        Assert.Equal(distance, position.DistanceTo(center), 9);
    }
}
