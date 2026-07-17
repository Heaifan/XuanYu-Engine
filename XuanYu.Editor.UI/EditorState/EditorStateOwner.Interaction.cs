namespace XuanYu.Editor.UI;

public sealed partial class EditorStateOwner
{
    EditorInteractionSnapshot _interaction = EditorInteractionSnapshot.Initial;
    long _nextInteractionSessionId = 1;

    public EditorInteractionSnapshot InteractionSnapshot => _interaction;

    public EditorInteractionChangedResult? Begin(BeginInteractionCommand command)
    {
        EnsureWriteThread();
        if (_interaction.HasCapture) return null;
        var old = _interaction;
        _interaction = new(old.Revision + 1, true, _nextInteractionSessionId++,
            command.OwnerTool, command.StartSnapshot, "", EditorInteractionPhase.Captured,
            command.Pointer);
        return InteractionChanged(old, EditorInteractionChangeKind.Began);
    }

    public EditorInteractionChangedResult? Preview(PreviewInteractionCommand command)
    {
        EnsureWriteThread();
        if (!EnsureCaptureOwner(command.SessionId, command.OwnerTool)) return null;
        if (!EnsurePointer(command.Pointer.PointerId)) return null;
        var old = _interaction;
        _interaction = old with
        {
            Revision = old.Revision + 1,
            Preview = command.Preview,
            Pointer = command.Pointer
        };
        return InteractionChanged(old, EditorInteractionChangeKind.Previewed);
    }

    public EditorInteractionChangedResult? Commit(CommitInteractionCommand command)
    {
        EnsureWriteThread();
        if (!EnsureCaptureOwner(command.SessionId, command.OwnerTool)) return null;
        if (!EnsurePointer(command.PointerId)) return null;
        var old = _interaction;
        _interaction = EditorInteractionSnapshot.Initial with { Revision = old.Revision + 1 };
        return InteractionChanged(old, EditorInteractionChangeKind.Committed);
    }

    public EditorInteractionChangedResult? Cancel(CancelInteractionCommand command)
    {
        EnsureWriteThread();
        if (!EnsureCaptureOwner(command.SessionId, command.OwnerTool)) return null;
        var old = _interaction;
        _interaction = EditorInteractionSnapshot.Initial with { Revision = old.Revision + 1 };
        return InteractionChanged(old, EditorInteractionChangeKind.Canceled);
    }

    bool EnsureCaptureOwner(long sessionId, string ownerTool)
    {
        if (!_interaction.HasCapture) return false;
        if (_interaction.SessionId == sessionId && _interaction.OwnerTool == ownerTool) return true;
        throw new InvalidOperationException("非捕获所有者不能修改当前交互事务。");
    }

    bool EnsurePointer(long pointerId)
    {
        if (_interaction.Pointer.IsEmpty && pointerId == 0) return true;
        return pointerId != 0 && _interaction.Pointer.PointerId == pointerId;
    }

    EditorInteractionChangedResult InteractionChanged(EditorInteractionSnapshot old, EditorInteractionChangeKind kind) =>
        new(old.Revision, _interaction.Revision, kind, old, _interaction);
}
