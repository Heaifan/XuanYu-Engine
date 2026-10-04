using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public enum DrawingCompletionReason
{
    Complete,
    TooFewPoints,
    InvalidGeometry
}

public sealed record DrawingCompletionEvaluation(
    bool CanComplete,
    CompletionPolicy Policy,
    DrawingCompletionReason Reason,
    int MinimumRequired,
    int CurrentCount,
    DrawingValidationFailureCategory ValidationFailureCategory)
{
    public static DrawingCompletionEvaluation FromValidation(
        DrawingPrimitiveKind kind, DrawingValidationResult validation) =>
        new(validation.IsValid, PolicyFor(kind),
            validation.IsValid ? DrawingCompletionReason.Complete :
            validation.FailureCategory == DrawingValidationFailureCategory.TooFewPoints
                ? DrawingCompletionReason.TooFewPoints : DrawingCompletionReason.InvalidGeometry,
            validation.MinimumRequired, validation.CurrentCount, validation.FailureCategory);

    static CompletionPolicy PolicyFor(DrawingPrimitiveKind kind) => kind == DrawingPrimitiveKind.Point
        ? CompletionPolicy.AutoCommit
        : CompletionPolicy.ManualComplete;
}

public static class DrawingCompletion
{
    public static DrawingCompletionEvaluation Evaluate(
        DrawingPrimitiveKind kind, IReadOnlyList<MapPoint>? points) =>
        DrawingCompletionEvaluation.FromValidation(kind, DrawingValidation.Validate(kind, points));
}
