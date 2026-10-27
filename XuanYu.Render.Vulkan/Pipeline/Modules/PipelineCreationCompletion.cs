using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static class VulkanPipelineCreationCompletion
{
    internal static void Rollback(Exception cause, params Action[] cleanup)
    {
        List<Exception>? failures = null;
        foreach (var release in cleanup)
        {
            try { release(); }
            catch (Exception failure) { (failures ??= []).Add(failure); }
        }
        if (failures is { Count: > 0 })
            throw new AggregateException("管线创建失败且资源回滚不完整。", [cause, .. failures]);
        ExceptionDispatchInfo.Capture(cause).Throw();
    }

    internal static T Complete<T>(T resource, Action<string>? log, Action rollback, params string[] messages)
    {
        try
        {
            foreach (var message in messages) log?.Invoke(message);
            return resource;
        }
        catch (Exception logFailure)
        {
            try { rollback(); }
            catch (Exception cleanupFailure)
            {
                throw new AggregateException("管线创建日志与资源回滚均失败。", logFailure, cleanupFailure);
            }
            throw;
        }
    }
}
