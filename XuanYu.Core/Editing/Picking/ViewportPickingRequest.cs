using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Picking;

public readonly record struct ViewportPickingRequest
{
    public ViewportPickingRequest(long requestSequence, ViewportState viewport, CameraState camera, double logicalX, double logicalY, SpatialQueryCategory mask, long spatialRevision)
    {
        if (requestSequence < 0) throw new ArgumentOutOfRangeException(nameof(requestSequence));
        if (!double.IsFinite(logicalX)) throw new ArgumentOutOfRangeException(nameof(logicalX));
        if (!double.IsFinite(logicalY)) throw new ArgumentOutOfRangeException(nameof(logicalY));
        if (spatialRevision < 0) throw new ArgumentOutOfRangeException(nameof(spatialRevision));

        RequestSequence = requestSequence;
        Viewport = viewport;
        Camera = camera;
        LogicalX = logicalX;
        LogicalY = logicalY;
        Mask = mask;
        SpatialRevision = spatialRevision;
    }

    public long RequestSequence { get; }

    public ViewportState Viewport { get; }

    public CameraState Camera { get; }

    public double LogicalX { get; }

    public double LogicalY { get; }

    public SpatialQueryCategory Mask { get; }

    public long SpatialRevision { get; }
}
