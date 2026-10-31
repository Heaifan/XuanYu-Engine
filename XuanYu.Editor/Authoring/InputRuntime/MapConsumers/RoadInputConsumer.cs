namespace XuanYu.Editor.Input.Map;

public sealed class RoadInputConsumer(IMapEditingInputBackend backend)
    : MapEditingInputConsumer(GestureOwner.Road, backend);
