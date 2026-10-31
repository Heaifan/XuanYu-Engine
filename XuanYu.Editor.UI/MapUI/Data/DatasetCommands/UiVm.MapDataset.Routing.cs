namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryRouteDatasetCommand(string name)
    {
        if (name == "新建数据集")
        {
            LogMapCommandReceived(name);
            _ = RunDatasetCommandAsync(name);
            return true;
        }
        if (name == "解除注册数据集")
        {
            LogMapCommandReceived(name);
            _ = RunDatasetCommandAsync(name);
            return true;
        }
        return false;
    }

    async Task RunDatasetCommandAsync(string name)
    {
        try
        {
            if (name == "新建数据集") await CreateDatasetAsync();
            else await UnregisterDatasetAsync();
        }
        catch (Exception ex)
        {
            DatasetFailed($"数据集操作失败：{ex.Message}", name == "新建数据集");
        }
    }
}
