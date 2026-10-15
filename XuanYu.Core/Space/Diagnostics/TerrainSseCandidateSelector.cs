namespace XuanYu.Core.Space.Diagnostics;

/// <summary>Selects an LOD from estimated pixel errors without depending on renderer state.</summary>
public static class TerrainSseCandidateSelector
{
    private static readonly double[] PixelBudgets = [0.5, 1, 2, 4];

    /// <summary>
    /// Chooses the coarsest candidate meeting a supported pixel budget. Pixel errors remain
    /// caller estimates; inside-chunk and near-plane cases fall back to the finest candidate.
    /// </summary>
    public static TerrainSseCandidateSelection Select(
        IReadOnlyList<TerrainSseLodMeasurement> measurements, double pixelBudget,
        int? previousLod = null, double hysteresisFraction = 0,
        bool cameraInsideChunk = false, bool nearPlaneIntersecting = false)
    {
        ArgumentNullException.ThrowIfNull(measurements);
        if (measurements.Count == 0) throw new ArgumentException("At least one LOD is required.", nameof(measurements));
        if (!PixelBudgets.Contains(pixelBudget)) throw new ArgumentOutOfRangeException(nameof(pixelBudget));
        if (previousLod.HasValue && previousLod.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(previousLod));
        if (!double.IsFinite(hysteresisFraction) || hysteresisFraction < 0 || hysteresisFraction >= 1)
            throw new ArgumentOutOfRangeException(nameof(hysteresisFraction));

        var ordered = measurements.OrderBy(m => m.Lod).ToArray();
        if (ordered.Any(m => m.Lod < 0) || ordered.Select(m => m.Lod).Distinct().Count() != ordered.Length)
            throw new ArgumentException("LOD values must be unique and non-negative.", nameof(measurements));
        var valid = ordered.Where(m => IsValid(m.EstimatedMaxPixels)).ToArray();
        var finest = ordered[0];
        if (cameraInsideChunk || nearPlaneIntersecting)
            return Result(finest, pixelBudget, cameraInsideChunk
                ? TerrainSseSelectionReason.CameraInsideChunkFallback
                : TerrainSseSelectionReason.NearPlaneIntersectionFallback);

        if (valid.Length != ordered.Length)
        {
            var previous = valid.FirstOrDefault(m => m.Lod == previousLod);
            var hasPrevious = previousLod.HasValue && valid.Any(m => m.Lod == previousLod.Value);
            return Result(hasPrevious ? previous : finest, pixelBudget,
                ordered.Any(m => m.EstimatedMaxPixels.HasValue && !IsValid(m.EstimatedMaxPixels))
                    ? TerrainSseSelectionReason.InvalidEstimateFallback
                    : TerrainSseSelectionReason.EstimateUnavailableFallback);
        }
        return SelectMeasured(valid, pixelBudget, previousLod, hysteresisFraction);
    }

    private static TerrainSseCandidateSelection SelectMeasured(TerrainSseLodMeasurement[] valid,
        double budget,
        int? previous, double hysteresis)
    {
        var within = valid.Where(m => m.EstimatedMaxPixels!.Value <= budget).ToArray();
        var baseline = within.Length == 0 ? valid[0] : within[^1];
        var reason = within.Length == 0 ? TerrainSseSelectionReason.AllEstimatedCandidatesOverBudget
            : TerrainSseSelectionReason.CoarsestWithinBudget;
        var previousMeasurement = valid.FirstOrDefault(m => m.Lod == previous);
        var hasPreviousMeasurement = previous.HasValue && valid.Any(m => m.Lod == previous.Value);
        if (hasPreviousMeasurement)
        {
            var oldError = previousMeasurement.EstimatedMaxPixels!.Value;
            var previousValue = previous.GetValueOrDefault();
            var upper = budget * (1 + hysteresis);
            if (oldError <= upper)
            {
                var lower = budget * (1 - hysteresis);
                var coarse = valid.Where(m => m.Lod > previousValue && m.EstimatedMaxPixels!.Value <= lower)
                    .ToArray();
                if (coarse.Length > 0)
                    return Result(coarse[^1], budget, TerrainSseSelectionReason.CoarsestWithinHysteresisThreshold);
                if (baseline.Lod != previousValue)
                    return Result(previousMeasurement, budget,
                        TerrainSseSelectionReason.HysteresisRetainedPreviousLod);
            }
        }
        return Result(baseline, budget, reason);
    }

    private static TerrainSseCandidateSelection Result(TerrainSseLodMeasurement measurement,
        double budget, TerrainSseSelectionReason reason)
    {
        var error = IsValid(measurement.EstimatedMaxPixels) ? measurement.EstimatedMaxPixels : null;
        var classification = !error.HasValue ?
            (measurement.EstimatedMaxPixels.HasValue ? TerrainSseErrorClassification.InvalidEstimate
                : TerrainSseErrorClassification.EstimateUnavailable) :
            error.Value <= budget ? TerrainSseErrorClassification.EstimatedWithinBudget
                : TerrainSseErrorClassification.EstimatedOverBudget;
        return new(measurement.Lod, error, reason, classification);
    }

    private static bool IsValid(double? error) => error.HasValue &&
        double.IsFinite(error.Value) && error.Value >= 0;
}
