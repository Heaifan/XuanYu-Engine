using System.Reflection;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class DrawingPreviewSnapTests
{
    [Fact]
    public void Preview_snapshot_describes_transient_geometry_without_authoring_count()
    {
        var preview = new DrawingPreviewSnapshot(
            new MapPoint(4, 5),
            new DrawingPreviewSegment(new MapPoint(1, 2), new MapPoint(4, 5)),
            new DrawingCloseCandidate(true),
            DrawingPreviewHoverState.Acceptable,
            true);

        Assert.Equal(new MapPoint(4, 5), preview.CursorPosition);
        Assert.Equal(new MapPoint(4, 5), preview.Segment!.End);
        Assert.True(preview.CloseCandidate.IsCandidate);
        Assert.DoesNotContain("PointCount", typeof(DrawingPreviewSnapshot)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name));
    }

    [Fact]
    public void Preview_snapshot_is_not_a_world_mutation_or_persistent_selection()
    {
        var preview = new DrawingPreviewSnapshot(null, null,
            new DrawingCloseCandidate(false), DrawingPreviewHoverState.None, false);

        Assert.Null(preview.CursorPosition);
        Assert.Null(preview.Segment);
        Assert.DoesNotContain(preview.GetType().GetProperties(), property =>
            property.Name.Contains("World", StringComparison.Ordinal) ||
            property.Name.Contains("Selection", StringComparison.Ordinal));
    }

    [Fact]
    public void Snap_candidate_is_position_only_and_does_not_carry_target_layer()
    {
        var candidate = new DrawingSnapCandidate(
            new MapPoint(10, 20), DrawingSnapKind.Vertex, "vertex-7", 2.5, 3, true);

        Assert.Equal(new MapPoint(10, 20), candidate.CandidatePosition);
        Assert.Equal(DrawingSnapKind.Vertex, candidate.Kind);
        Assert.DoesNotContain(candidate.GetType().GetProperties(), property =>
            property.Name.Contains("Layer", StringComparison.Ordinal) ||
            property.Name.Contains("Dataset", StringComparison.Ordinal));
    }

    [Fact]
    public void Snap_candidate_has_no_region_road_or_marker_authority()
    {
        var propertyNames = typeof(DrawingSnapCandidate)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name);

        Assert.DoesNotContain(propertyNames, name =>
            name.Contains("Region", StringComparison.Ordinal) ||
            name.Contains("Road", StringComparison.Ordinal) ||
            name.Contains("Marker", StringComparison.Ordinal));
    }

    [Fact]
    public void Close_candidate_is_preview_completion_information_only()
    {
        var close = new DrawingCloseCandidate(true);
        Assert.True(close.IsCandidate);
        Assert.DoesNotContain(close.GetType().GetProperties(), property =>
            property.Name.Contains("Commit", StringComparison.Ordinal) ||
            property.Name.Contains("History", StringComparison.Ordinal));
    }

    [Fact]
    public void Preview_and_snap_contracts_have_no_forbidden_runtime_dependencies()
    {
        var references = typeof(DrawingPreviewSnapshot).Assembly.GetReferencedAssemblies();

        Assert.DoesNotContain(references, assembly =>
            assembly.Name?.StartsWith("Avalonia", StringComparison.Ordinal) == true ||
            assembly.Name?.Contains("Render", StringComparison.Ordinal) == true ||
            assembly.Name?.Contains("Terrain", StringComparison.Ordinal) == true);
    }
}
