using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public enum DrawingValidationFailureCategory
{
    None,
    TooFewPoints,
    NonFinitePoint,
    AdjacentDuplicatePoint
}

public sealed record DrawingValidationResult(
    bool IsValid,
    DrawingValidationFailureCategory FailureCategory,
    int MinimumRequired,
    int CurrentCount)
{
    public static DrawingValidationResult Valid(int minimum, int current) =>
        new(true, DrawingValidationFailureCategory.None, minimum, current);

    public static DrawingValidationResult Invalid(
        DrawingValidationFailureCategory category, int minimum, int current) =>
        new(false, category, minimum, current);
}

public static class DrawingValidation
{
    public static DrawingValidationResult Validate(
        DrawingPrimitiveKind kind, IReadOnlyList<MapPoint>? points)
    {
        var input = points ?? Array.Empty<MapPoint>();
        var minimum = MinimumRequired(kind);
        for (var index = 0; index < input.Count; index++)
        {
            if (!double.IsFinite(input[index].X) || !double.IsFinite(input[index].Y))
                return DrawingValidationResult.Invalid(
                    DrawingValidationFailureCategory.NonFinitePoint, minimum, input.Count);
            if (index > 0 && input[index] == input[index - 1])
                return DrawingValidationResult.Invalid(
                    DrawingValidationFailureCategory.AdjacentDuplicatePoint, minimum, input.Count);
        }
        if (input.Count < minimum)
            return DrawingValidationResult.Invalid(
                DrawingValidationFailureCategory.TooFewPoints, minimum, input.Count);
        return DrawingValidationResult.Valid(minimum, input.Count);
    }

    internal static int MinimumRequired(DrawingPrimitiveKind kind) => kind switch
    {
        DrawingPrimitiveKind.Point => 1,
        DrawingPrimitiveKind.Polyline => 2,
        DrawingPrimitiveKind.Polygon => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
}
