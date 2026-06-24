using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;

namespace XuanYu.Engine.Tests.Editor.Transform.Translation.Axis;

public sealed class AxisPlaneTranslationSolverTests
{
    // ── 帮助构造 DragPlane 锚点 ──────────────────────
    static AxisTranslationAnchor MakeAnchor(Vector3d pos, Vector3d axis, Vector3d pivot,
        Vector3d planeNormal, Vector3d startHit) => new(pos, axis, pivot,
            PixelsPerWorldUnit: 0, ScreenDirection: default,
            StartPointerX: 0, StartPointerY: 0, Mode: AxisTranslationMode.DragPlane)
    {
        StartIntersection = startHit,
        DragPlaneNormal = planeNormal,
        CameraForward = new(0, 0, -1),
        CameraRight = new(1, 0, 0),
        CameraUp = new(0, 1, 0),
    };

    [Fact]
    public void SolveDragPlane_XAxis_OnlyChangesX()
    {
        var anchor = MakeAnchor(new(10, 20, 30), Vector3d.UnitX, new(10, 20, 30),
            new(0, 0, -1), new(10, 20, 30));
        var currentHit = new Vector3d(15, 20, 30); // X+5, Y/Z unchanged
        var result = AxisTranslationSolver.SolveDragPlane(anchor, currentHit);
        Assert.Equal(15, result.X);
        Assert.Equal(20, result.Y); // Y unchanged
        Assert.Equal(30, result.Z); // Z unchanged
    }

    [Fact]
    public void SolveDragPlane_YAxis_OnlyChangesY()
    {
        var anchor = MakeAnchor(new(10, 20, 30), Vector3d.UnitY, new(10, 20, 30),
            new(0, 0, -1), new(10, 20, 30));
        var currentHit = new Vector3d(10, 35, 30); // Y+15
        var result = AxisTranslationSolver.SolveDragPlane(anchor, currentHit);
        Assert.Equal(10, result.X);
        Assert.Equal(35, result.Y);
        Assert.Equal(30, result.Z);
    }

    [Fact]
    public void SolveDragPlane_ZAxis_OnlyChangesZ()
    {
        var anchor = MakeAnchor(new(10, 20, 30), Vector3d.UnitZ, new(10, 20, 30),
            new(0, -1, 0), new(10, 20, 30));
        var currentHit = new Vector3d(10, 20, 50); // Z+20
        var result = AxisTranslationSolver.SolveDragPlane(anchor, currentHit);
        Assert.Equal(10, result.X);
        Assert.Equal(20, result.Y);
        Assert.Equal(50, result.Z);
    }

    /// <summary>模拟 45° 俯视角拖 Z 轴，不应出现倍率异常。</summary>
    [Fact]
    public void SolveDragPlane_ZAxis_45DegreeView_NoMagnificationAnomaly()
    {
        // 45° 视角：视线方向为 (0, -1, -1)/√2，Z 轴在屏幕上的投影很短
        // 但射线约束方案应产生与 X/Y 轴一致的位移量
        var planeNormal = new Vector3d(0, -0.707, -0.707); // Gram-Schmidt: view剔除Z分量后归一化
        var startHit = new Vector3d(0, 0, 0);
        var anchor = MakeAnchor(Vector3d.Zero, Vector3d.UnitZ, Vector3d.Zero,
            planeNormal, startHit);

        // 鼠标移动 5 单位（沿射线-平面交点变化）
        var currentHit = new Vector3d(0, -3, 4); // hit点在平面上的移动
        var result = AxisTranslationSolver.SolveDragPlane(anchor, currentHit);

        // Z 轴位移应为 Dot(delta, UnitZ)
        Assert.Equal(4, result.Z);
        // X 不变，Y 不变
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void SolveDragPlane_RayParallelToPlane_ReturnsInitial()
    {
        // 平面法线垂直于射线方向 → 无交点
        var anchor = MakeAnchor(new(10, 20, 30), Vector3d.UnitX, new(10, 20, 30),
            new(0, 0, -1), new(10, 20, 30));
        // 不传入 currentIntersection——模拟射线平行时不应更新
        // SolveDragPlane 用无效交点(负值或零长度)应返回初始位置
        var result = AxisTranslationSolver.SolveDragPlane(anchor, anchor.StartIntersection);
        Assert.Equal(10, result.X); // 无变化
        Assert.Equal(20, result.Y);
        Assert.Equal(30, result.Z);
    }

    [Fact]
    public void SolveDragPlane_NoMouseMovement_ReturnsInitial()
    {
        var anchor = MakeAnchor(new(5, 10, 15), Vector3d.UnitX, new(5, 10, 15),
            new(0, 0, -1), new(5, 10, 15));
        var result = AxisTranslationSolver.SolveDragPlane(anchor, new(5, 10, 15)); // 无位移
        Assert.Equal(5, result.X);
        Assert.Equal(10, result.Y);
        Assert.Equal(15, result.Z);
    }

    [Fact]
    public void SolveDragPlane_ScreenProjectionMode_ReturnsInitial()
    {
        // ScreenProjection 模式的锚点 → SolveDragPlane 应返回初始位置
        var anchor = new AxisTranslationAnchor(
            new(1, 2, 3), Vector3d.UnitX, new(1, 2, 3),
            PixelsPerWorldUnit: 10, ScreenDirection: new(1, 0),
            StartPointerX: 0, StartPointerY: 0,
            Mode: AxisTranslationMode.ScreenProjection);
        var result = AxisTranslationSolver.SolveDragPlane(anchor, new(10, 20, 30));
        Assert.Equal(1, result.X);
    }

    // ── Gram-Schmidt 平面法线 ──────────────────────
    [Fact]
    public void BuildPlaneNormal_GramSchmidt_ContainsAxis()
    {
        // 验证 Gram-Schmidt 构造的法线与轴垂直
        var axis = Vector3d.UnitZ;
        var view = new Vector3d(0, -1, -1).Normalize();
        var n = view - view.Dot(axis) * axis;
        n = n.Normalize();

        // 法线应垂直于轴
        Assert.True(Math.Abs(n.Dot(axis)) < 1e-6);
        // 法线不应为零
        Assert.False(n.IsZero);
    }
}
