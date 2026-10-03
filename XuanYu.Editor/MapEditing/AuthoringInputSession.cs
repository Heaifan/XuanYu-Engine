using XuanYu.Editor.Input;

namespace XuanYu.Editor.MapEditing;

public enum AuthoringInputKind
{
    Region,
    Road,
}

public sealed record AuthoringInputSnapshot(bool IsActive, AuthoringInputKind? Kind, GestureOwner Owner)
{
    public static AuthoringInputSnapshot Initial { get; } =
        new(false, null, GestureOwner.None);
}

public sealed class AuthoringInputSession
{
    public bool IsActive { get; private set; }
    public AuthoringInputKind? Kind { get; private set; }
    public GestureOwner Owner => Kind == AuthoringInputKind.Region ? GestureOwner.Region
        : Kind == AuthoringInputKind.Road ? GestureOwner.Road : GestureOwner.None;
    public AuthoringInputSnapshot Snapshot => new(IsActive, Kind, Owner);

    public bool Begin(AuthoringInputKind kind)
    {
        if (IsActive) return false;
        Kind = kind;
        IsActive = true;
        return true;
    }

    public bool End()
    {
        if (!IsActive) return false;
        IsActive = false;
        Kind = null;
        return true;
    }
}
