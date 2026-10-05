using System.Collections.Immutable;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// Legacy render compatibility only. Authoring belongs to RoadDrawingController.
public sealed class RoadDrawingState
{
    bool _isActive;
    public MapRoadDraft? Draft { get; private set; }
    public MapPoint? Cursor { get; private set; }
    public bool IsActive => _isActive;
    public bool CanUndo { get; private set; }
    public bool CanRedo { get; private set; }

    public void ProjectFrom(DrawingSession? session, RoadDrawingMetadata? metadata)
    {
        if (session is null || metadata is null || !session.IsActive)
        {
            Clear();
            return;
        }
        Draft = new(metadata.LayerId, metadata.DisplayName, metadata.Kind,
            session.Draft.Points.ToImmutableArray());
        Cursor = session.Preview?.CursorPosition;
        _isActive = true;
        CanUndo = session.CanUndo;
        CanRedo = session.CanRedo;
    }

    public void Clear()
    {
        _isActive = false; Draft = null; Cursor = null; CanUndo = false; CanRedo = false;
    }
}
