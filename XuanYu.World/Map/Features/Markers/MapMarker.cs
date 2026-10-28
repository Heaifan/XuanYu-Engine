namespace XuanYu.World.Map;

public sealed record MapMarker(
    MapMarkerId MarkerId,
    MapLayerId LayerId,
    string DisplayName,
    MapPoint Position,
    bool IsVisible = true,
    bool IsLocked = false);
