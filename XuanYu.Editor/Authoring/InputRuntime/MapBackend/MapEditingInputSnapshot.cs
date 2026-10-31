namespace XuanYu.Editor.Input.Map;

public enum MapEditingInputPhase { Idle, Preview }
public enum MapEditingInputTerminal { None, Committed, Canceled }

public readonly record struct MapEditingInputSnapshot(
    MapEditingInputPhase Phase, MapEditingInputTerminal LastTerminal,
    bool HasPreview, bool HasSnapCandidate, bool HasPressedState,
    bool HasTemporaryGeometry, bool HasTransaction)
{
    public static MapEditingInputSnapshot Idle => new(
        MapEditingInputPhase.Idle, MapEditingInputTerminal.None, false, false, false, false, false);
}
