namespace XuanYu.Editor.Input.Map;

public sealed class MarkerInputConsumer(IMapEditingInputBackend backend)
    : MapEditingInputConsumer(GestureOwner.Marker, backend);
