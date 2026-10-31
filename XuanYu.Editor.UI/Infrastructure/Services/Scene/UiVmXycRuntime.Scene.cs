namespace XuanYu.Editor.UI;

using XuanYu.Editor.UI.XycControl;

sealed partial class UiVmXycRuntime : IXycSceneRuntime
{
    public string SceneInstanceId => InstanceId;
    public XycSceneState InspectTestScene() => OnUi(vm.InspectXycTestScene);
    public XycEntity[] ListSceneEntities() => OnUi(vm.ListXycSceneEntities);
    public XycEntity InspectSceneEntity(string entityId) => OnUi(() => vm.InspectXycSceneEntity(entityId));
    public XycSceneState CreateTestCube(string sceneId, long revision) =>
        OnUi(() => vm.CreateXycTestCube(sceneId, revision));
    public XycSceneState SetTestPosition(string sceneId, long revision, string entityId, Vector3 position) =>
        OnUi(() => vm.SetXycTestPosition(sceneId, revision, entityId, position));
    public XycSceneState UndoTestOperation(string sceneId, long revision, string entityId) =>
        OnUi(() => vm.UndoXycTestOperation(sceneId, revision, entityId));
    public XycSceneState RedoTestOperation(string sceneId, long revision, string entityId) =>
        OnUi(() => vm.RedoXycTestOperation(sceneId, revision, entityId));
}
