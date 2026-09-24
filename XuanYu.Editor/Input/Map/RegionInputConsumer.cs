namespace XuanYu.Editor.Input.Map;

public sealed class RegionInputConsumer(IMapEditingInputBackend backend)
    : MapEditingInputConsumer(GestureOwner.Region, backend);
