namespace XuanYu.Core.Space.Diagnostics;

/// <summary>LOD error measured in pixels by a caller-provided projection estimate.</summary>
public readonly record struct TerrainSseLodMeasurement(int Lod, double? EstimatedMaxPixels);

public enum TerrainSseErrorClassification
{
    EstimatedWithinBudget,
    EstimatedOverBudget,
    EstimateUnavailable,
    InvalidEstimate
}

public enum TerrainSseSelectionReason
{
    CoarsestWithinBudget,
    CoarsestWithinHysteresisThreshold,
    HysteresisRetainedPreviousLod,
    AllEstimatedCandidatesOverBudget,
    EstimateUnavailableFallback,
    InvalidEstimateFallback,
    CameraInsideChunkFallback,
    NearPlaneIntersectionFallback
}

/// <summary>
/// Candidate decision. EstimatedMaxPixels is an estimate, never a guaranteed screen-space bound.
/// </summary>
public sealed record TerrainSseCandidateSelection(int ChosenLod, double? EstimatedMaxPixels,
    TerrainSseSelectionReason Reason, TerrainSseErrorClassification Classification);
