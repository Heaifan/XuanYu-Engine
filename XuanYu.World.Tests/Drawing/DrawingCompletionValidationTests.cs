using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class DrawingCompletionValidationTests
{
    [Fact]
    public void Point_with_one_valid_input_is_auto_commit_complete()
    {
        var result = DrawingCompletion.Evaluate(DrawingPrimitiveKind.Point, [new MapPoint(1, 2)]);
        Assert.True(result.CanComplete);
        Assert.Equal(CompletionPolicy.AutoCommit, result.Policy);
    }

    [Fact]
    public void Polyline_requires_two_points_and_manual_completion()
    {
        var incomplete = DrawingCompletion.Evaluate(DrawingPrimitiveKind.Polyline, [new MapPoint(1, 2)]);
        var complete = DrawingCompletion.Evaluate(DrawingPrimitiveKind.Polyline,
            [new MapPoint(1, 2), new MapPoint(3, 4)]);
        Assert.False(incomplete.CanComplete);
        Assert.Equal(2, incomplete.MinimumRequired);
        Assert.True(complete.CanComplete);
        Assert.Equal(CompletionPolicy.ManualComplete, complete.Policy);
    }

    [Fact]
    public void Polygon_requires_three_points_and_manual_completion()
    {
        var incomplete = DrawingCompletion.Evaluate(DrawingPrimitiveKind.Polygon,
            [new MapPoint(1, 2), new MapPoint(3, 4)]);
        var complete = DrawingCompletion.Evaluate(DrawingPrimitiveKind.Polygon,
            [new MapPoint(1, 2), new MapPoint(3, 4), new MapPoint(5, 6)]);
        Assert.False(incomplete.CanComplete);
        Assert.Equal(3, incomplete.MinimumRequired);
        Assert.True(complete.CanComplete);
    }

    [Fact]
    public void Primitive_validation_is_structured_and_domain_agnostic()
    {
        var result = DrawingValidation.Validate(DrawingPrimitiveKind.Polyline, [new MapPoint(1, 2)]);
        Assert.False(result.IsValid);
        Assert.Equal(DrawingValidationFailureCategory.TooFewPoints, result.FailureCategory);
        Assert.Equal(2, result.MinimumRequired);
        Assert.Equal(1, result.CurrentCount);
        Assert.DoesNotContain("Region", result.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain("Road", result.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain("Marker", result.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Invalid_geometry_prevents_completion_without_ui_or_runtime_dependencies()
    {
        var result = DrawingCompletion.Evaluate(DrawingPrimitiveKind.Polygon,
            [new MapPoint(1, 2), new MapPoint(double.NaN, 4), new MapPoint(5, 6)]);
        Assert.False(result.CanComplete);
        Assert.Equal(DrawingValidationFailureCategory.NonFinitePoint, result.ValidationFailureCategory);
        Assert.DoesNotContain(typeof(DrawingCompletion).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name is "Avalonia" or "Silk.NET.Vulkan");
    }
}
