namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryRouteDatasetCommand(string name)
    {
        if (name == "新建数据集")
        {
            LogMapCommandReceived(name);
            _ = CreateDatasetAsync();
            return true;
        }
        if (name == "解除注册数据集")
        {
            LogMapCommandReceived(name);
            _ = UnregisterDatasetAsync();
            return true;
        }
        return false;
    }
}
