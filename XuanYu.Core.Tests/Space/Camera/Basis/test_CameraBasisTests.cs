using XuanYu.Core.Math;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

// F3-F2：唯一相机正交基生成器合同——成功结果必须三轴单位正交，失败必须明确原因。
public sealed class CameraBasisTests
{
    static readonly Vector3d Center = new(3, -2, 5);
    static readonly Vector3d SlantPos = new(10, 10, 10);

    [Fact]
    public void Slant_view_with_valid_preferred_up_keeps_it()
    {
        AssertTrue(Try(SlantPos, Center, Vector3d.UnitZ, out var forward, out var right, out var up, out _));
        // 斜视下 up 经正交化会自然倾斜（保持右手系），断言三轴单位正交即可。
        Assert.True(System.Math.Abs(forward.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(right.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(up.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(forward.Dot(up)) < 1e-9);
        Assert.True(System.Math.Abs(forward.Dot(right)) < 1e-9);
        Assert.True(System.Math.Abs(right.Dot(up)) < 1e-9);
        Assert.True(up.Dot(Vector3d.UnitZ) > 0.5); // 仍保持 Z 向上的主导语义
    }

    [Fact]
    public void Zero_preferred_up_falls_back_to_world_axis()
    {
        AssertTrue(Try(SlantPos, Center, Vector3d.Zero, out _, out _, out _, out _));
    }

    [Fact]
    public void Nan_preferred_up_falls_back_to_world_axis()
    {
        AssertTrue(Try(SlantPos, Center, new Vector3d(double.NaN, 0, 0), out _, out _, out _, out _));
    }

    [Theory]
    [InlineData(0.999999999)] // 与 Forward 平行（顶视）
    [InlineData(0.99)]        // 近似平行 > ParallelLimit
    public void Parallel_preferred_up_falls_back_to_world_axis(double forwardZ)
    {
        var forward = new Vector3d(0, 0, forwardZ).Normalize();
        var position = Center - (forward * 8.0);
        AssertTrue(Try(position, Center, Vector3d.UnitZ, out var outF, out _, out var outUp, out _));
        Assert.True(System.Math.Abs(outF.Dot(outUp)) < 0.2); // 回退后不得再平行
    }

    [Fact]
    public void Top_view_preferred_up_plus_y_is_kept()
    {
        var position = Center + new Vector3d(0, 0, 8);
        AssertTrue(Try(position, Center, new Vector3d(0, 1, 0), out var forward, out var right, out var up, out _));
        Assert.Equal(new Vector3d(0, 0, -1), forward);
        Assert.True(right.Dot(new Vector3d(1, 0, 0)) > 0.99);
        Assert.True(up.Dot(new Vector3d(0, 1, 0)) > 0.99);
    }

    [Fact]
    public void Bottom_view_preferred_up_minus_y_is_kept()
    {
        var position = Center - new Vector3d(0, 0, 8);
        AssertTrue(Try(position, Center, new Vector3d(0, -1, 0), out var forward, out var right, out var up, out _));
        Assert.Equal(new Vector3d(0, 0, 1), forward);
        Assert.True(right.Dot(new Vector3d(1, 0, 0)) > 0.99); // 无镜像
        Assert.True(up.Dot(new Vector3d(0, -1, 0)) > 0.99);
    }

    [Fact]
    public void Position_equal_to_center_fails_with_reason()
    {
        Assert.False(Try(Center, Center, Vector3d.UnitZ, out _, out _, out _, out var reason));
        Assert.NotEmpty(reason);
    }

    [Fact]
    public void Huge_finite_coordinates_still_orthonormal()
    {
        var huge = new Vector3d(1e9, -1e9, 1e9);
        var pos = huge + new Vector3d(1e6, 0, 0);
        AssertTrue(Try(pos, huge, Vector3d.UnitZ, out var f, out var r, out var u, out _));
        Assert.True(System.Math.Abs(f.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(r.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(u.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(f.Dot(r)) < 1e-9 && System.Math.Abs(f.Dot(u)) < 1e-9 &&
                    System.Math.Abs(r.Dot(u)) < 1e-9);
    }

    static bool Try(Vector3d position, Vector3d center, Vector3d preferredUp,
        out Vector3d forward, out Vector3d right, out Vector3d up, out string reason) =>
        CameraBasis.TryCreate(position, center, preferredUp, out forward, out right, out up, out reason);

    static void AssertTrue(bool condition) => Xunit.Assert.True(condition);
}
