using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

public sealed record XycRequest(string SchemaVersion, string RequestId,
    string SessionId, string Command, JsonElement Args);

public sealed record XycCamera(Vector3 Position, Vector3 Forward, Vector3 Up,
    double FovDegrees, double NearPlane, double FarPlane, string Mode,
    double OrthographicScale, Vector3 ObservationCenter, long Revision);

public sealed record XycOrbitDelta(double DeltaX, double DeltaY);

public sealed record Vector3(double X, double Y, double Z);

public sealed record XycResponse(string RequestId, bool Success, string? ErrorCode,
    object? ObservedState, long? SceneRevision, long? CameraRevision,
    double DurationMs, string? EvidencePath = null);

public sealed record XycRuntimeInfo(string ProtocolVersion, string InstanceId,
    string IsolationMode, string RenderCapability, string Build, string Branch,
    string Dirty, int ProcessId = 0, long ViewportFrameId = 0);

public sealed record XycScene(string SceneId, long SceneRevision,
    string TerrainState, int EntityCount, string? TerrainId = null,
    int? TerrainRevision = null, string? ScaleLabel = null,
    long? ViewportFrameId = null, int? ViewportWidth = null,
    int? ViewportHeight = null, long? PresentedFrameId = null,
    long? PresentedCameraRevision = null, uint? PresentedImageIndex = null,
    bool? PresentedCaptureHeld = null);

public sealed record XycSessionClosed(bool Closed);

public sealed record XycCaptureState(bool Armed);

public sealed record XycCaptureRequest(long CameraRevision);
