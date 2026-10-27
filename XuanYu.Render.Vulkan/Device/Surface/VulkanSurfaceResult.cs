namespace XuanYu.Render.Vulkan;

// VK3-B2：Vulkan Surface 创建结果。成功状态由 Owner 是否存在唯一决定。
public sealed record VulkanSurfaceResult
{
    public VulkanSurfaceResult(bool Success, VulkanSurfaceOwner? Owner,
        string ErrorType = "", string ErrorMessage = "")
    {
        if (Success != (Owner is not null))
            throw new ArgumentException("Success must match whether Owner exists.", nameof(Success));
        this.Owner = Owner;
        this.ErrorType = ErrorType;
        this.ErrorMessage = ErrorMessage;
    }

    public bool Success => Owner is not null;
    public VulkanSurfaceOwner? Owner { get; init; }
    public string ErrorType { get; init; }
    public string ErrorMessage { get; init; }

    public void Deconstruct(out bool Success, out VulkanSurfaceOwner? Owner,
        out string ErrorType, out string ErrorMessage)
    {
        Success = this.Success;
        Owner = this.Owner;
        ErrorType = this.ErrorType;
        ErrorMessage = this.ErrorMessage;
    }
}
