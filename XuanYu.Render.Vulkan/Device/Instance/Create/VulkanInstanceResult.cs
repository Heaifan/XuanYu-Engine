namespace XuanYu.Render.Vulkan;

// VK3-B1：Vulkan Instance 创建结果。成功状态由 Owner 是否存在唯一决定。
public sealed record VulkanInstanceResult
{
    public VulkanInstanceResult(bool Success, VulkanInstanceOwner? Owner, uint ApiVersion,
        string ErrorType = "", string ErrorMessage = "")
    {
        if (Success != (Owner is not null))
            throw new ArgumentException("Success must match whether Owner exists.", nameof(Success));
        this.Owner = Owner;
        this.ApiVersion = ApiVersion;
        this.ErrorType = ErrorType;
        this.ErrorMessage = ErrorMessage;
    }

    public bool Success => Owner is not null;
    public VulkanInstanceOwner? Owner { get; init; }
    public uint ApiVersion { get; init; }
    public string ErrorType { get; init; }
    public string ErrorMessage { get; init; }

    public void Deconstruct(out bool Success, out VulkanInstanceOwner? Owner,
        out uint ApiVersion, out string ErrorType, out string ErrorMessage)
    {
        Success = this.Success;
        Owner = this.Owner;
        ApiVersion = this.ApiVersion;
        ErrorType = this.ErrorType;
        ErrorMessage = this.ErrorMessage;
    }
}
