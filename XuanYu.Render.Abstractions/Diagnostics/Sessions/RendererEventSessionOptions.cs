namespace XuanYu.Render.Abstractions.Diagnostics;

public sealed record RendererEventSessionOptions(
    string Directory,
    string RunId,
    string BuildIdentity,
    bool ProbeEnabled,
    int RuntimeCapacity = 512,
    int ProbeCapacity = 128,
    long SegmentBytes = 8 * 1024 * 1024,
    int RetainedSegments = 3);
