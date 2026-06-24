using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;

namespace XuanYu.Engine.Tests.Editor.Transform.Translation.Axis;

/// <summary>
/// 关键验收测试：1 次与 100 次 PointerMove 必须产生完全相同的结果。
/// </summary>
public sealed class AxisTranslationEventCountTests
{
    private static readonly AxisTranslationAnchor TestAnchor = new(
        InitialPosition: new Vector3d(100, 200, 30),
        Axis: Vector3d.UnitX,
        Pivot: new Vector3d(100, 200, 0),
        PixelsPerWorldUnit: 50.0, // 50 pixels = 1 world unit
        ScreenDirection: new Vector2d(1, 0),
        StartPointerX: 400,
        StartPointerY: 300,
        Mode: AxisTranslationMode.ScreenProjection);

    [Fact]
    public void OneEvent_Vs_HundredEvents_SameResult()
    {
        // 1 event: directly from (400,300) to (500,300) = +100px X
        var result1 = AxisTranslationSolver.Solve(TestAnchor, 500, 300);

        // 100 events: stepping 1px at a time
        var resultN = TestAnchor.InitialPosition;
        for (int i = 0; i < 100; i++)
        {
            resultN = AxisTranslationSolver.Solve(TestAnchor, 400 + (i + 1), 300);
        }

        // 100 events should reach exactly the same position as 1 event
        Assert.Equal(result1.X, resultN.X, 10);
        Assert.Equal(result1.Y, resultN.Y, 10);
        Assert.Equal(result1.Z, resultN.Z, 10);
    }

    [Fact]
    public void NoPointerMove_KeepsPosition()
    {
        var result = AxisTranslationSolver.Solve(TestAnchor, 400, 300);
        Assert.Equal(100, result.X, 3);
        Assert.Equal(200, result.Y, 3);
        Assert.Equal(30, result.Z, 3);
    }

    [Fact]
    public void Solve_OneHundredPixelRight_MovesTwoUnits()
    {
        var result = AxisTranslationSolver.Solve(TestAnchor, 500, 300);
        Assert.Equal(102, result.X, 3); // 100 + 100/50 = 102
        Assert.Equal(200, result.Y, 3);
        Assert.Equal(30, result.Z, 3);
    }

    [Fact]
    public void Solve_XAxis_OnlyChangesX()
    {
        var result = AxisTranslationSolver.Solve(TestAnchor, 500, 350);
        Assert.Equal(102, result.X, 3); // X changes
        Assert.Equal(200, result.Y, 3); // Y unchanged
        Assert.Equal(30, result.Z, 3);  // Z unchanged
    }

    [Fact]
    public void Solve_YAxis_OnlyChangesY()
    {
        var anchor = TestAnchor with
        {
            Axis = Vector3d.UnitY,
            ScreenDirection = new Vector2d(0, -1),
        };
        var result = AxisTranslationSolver.Solve(anchor, 400, 200); // up 100px
        Assert.Equal(100, result.X, 3); // X unchanged
        Assert.Equal(202, result.Y, 3); // Y + 100/50 = +2
        Assert.Equal(30, result.Z, 3);  // Z unchanged
    }

    [Fact]
    public void Solve_ZAxis_OnlyChangesZ()
    {
        var anchor = TestAnchor with
        {
            Axis = Vector3d.UnitZ,
            ScreenDirection = new Vector2d(0, -1),
        };
        var result = AxisTranslationSolver.Solve(anchor, 400, 200); // up 100px
        Assert.Equal(100, result.X, 3); // X unchanged
        Assert.Equal(200, result.Y, 3); // Y unchanged
        Assert.Equal(32, result.Z, 3);  // Z + 100/50 = +2
    }

    [Fact]
    public void DisabledMode_ReturnsInitialPosition()
    {
        var anchor = TestAnchor with { Mode = AxisTranslationMode.Disabled };
        var result = AxisTranslationSolver.Solve(anchor, 900, 900);
        Assert.Equal(TestAnchor.InitialPosition, result);
    }

    [Fact]
    public void InvalidAnchor_ReturnsInitialPosition()
    {
        var anchor = TestAnchor with { PixelsPerWorldUnit = 0 };
        var result = AxisTranslationSolver.Solve(anchor, 500, 300);
        Assert.Equal(TestAnchor.InitialPosition, result);
    }
}
