using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor;

public sealed class EditorAuthoringOwner
{
    readonly AuthoringInputSession _session = new();
    public AuthoringInputSnapshot Snapshot => _session.Snapshot;

    public AuthoringInputSnapshot Begin(BeginAuthoringInputCommand command)
    {
        _session.Begin(command.Kind);
        return _session.Snapshot;
    }

    public AuthoringInputSnapshot End(EndAuthoringInputCommand command)
    {
        _session.End();
        return _session.Snapshot;
    }
}
