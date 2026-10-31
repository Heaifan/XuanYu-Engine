namespace XuanYu.Editor.Input.Map;

public sealed class MapGeometryInputConsumer(IMapEditingInputBackend backend)
    : MapEditingInputConsumer(GestureOwner.MapEdit, backend);
