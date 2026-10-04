namespace XuanYu.Editor.Drawing;

public enum DrawingState
{
    Idle,
    Armed,
    Drawing,
    Committing,
    Blocked,
    Completed,
    Cancelled,
    Aborted
}

public enum DrawingInputDisposition
{
    Accepted,
    Rejected,
    Blocked
}

public enum CompletionPolicy
{
    ManualComplete,
    AutoCommit
}

public enum PostCommitPolicy
{
    Exit,
    RestartArmed
}

public enum RecoveryPolicy
{
    Retryable,
    PreserveDraft,
    Abort
}

public enum DrawingCommitFailurePolicy
{
    PreserveDraft,
    Blocked,
    Abort
}

public enum SnapState
{
    Inactive,
    Available,
    Active
}
