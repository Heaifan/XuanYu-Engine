using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public sealed class DrawingCoordinator
{
    public DrawingSession? ActiveSession { get; private set; }

    public DrawingSession Begin(
        DrawingPrimitiveKind primitiveKind, string toolIdentity, string domainIdentity)
    {
        Terminate();
        return ActiveSession = new DrawingSession(primitiveKind, toolIdentity, domainIdentity);
    }

    public void UpdatePreview(MapPoint cursor, DrawingSnapCandidate? candidate) =>
        ActiveSession?.UpdatePreview(cursor, candidate);

    public DrawingInputResult AcceptInput(MapPoint point) => ActiveSession?.AcceptInput(point) ??
        DrawingInputResult.Blocked("NoActiveSession", "绘制会话不可用。", RecoveryPolicy.Abort);

    public DrawingCommitRequest? Complete()
    {
        var request = ActiveSession?.Complete();
        return request;
    }

    public bool Cancel()
    {
        if (ActiveSession is null) return false;
        ActiveSession.Cancel(); ActiveSession = null; return true;
    }

    public void Terminate()
    {
        ActiveSession?.Terminate(); ActiveSession = null;
    }
}
