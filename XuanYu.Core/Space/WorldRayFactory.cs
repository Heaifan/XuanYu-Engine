using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public static class WorldRayFactory
{
    public static WorldRay FromViewportPoint(
        ViewProjectionState state,
        double logicalX,
        double logicalY)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (!double.IsFinite(logicalX)) throw new ArgumentOutOfRangeException(nameof(logicalX));
        if (!double.IsFinite(logicalY)) throw new ArgumentOutOfRangeException(nameof(logicalY));

        var ndcX = ToNdcX(state.Viewport, logicalX);
        var ndcY = ToNdcY(state.Viewport, logicalY);
        var near = state.TransformPointToWorld(ndcX, ndcY, 0.0);
        var far = state.TransformPointToWorld(ndcX, ndcY, 1.0);
        var direction = far - near;

        return new WorldRay(near, direction);
    }

    static double ToNdcX(ViewportState viewport, double logicalX)
    {
        return ((logicalX - viewport.LogicalX) / viewport.LogicalWidth * 2.0) - 1.0;
    }

    static double ToNdcY(ViewportState viewport, double logicalY)
    {
        return 1.0 - ((logicalY - viewport.LogicalY) / viewport.LogicalHeight * 2.0);
    }
}
