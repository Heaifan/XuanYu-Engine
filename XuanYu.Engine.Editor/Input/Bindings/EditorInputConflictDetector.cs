namespace FluidWarfare.Editor.Input.Bindings;

/// <summary>
/// 绑定冲突检测。
/// 同一上下文内相同手势不能绑定多个动作。
/// </summary>
public static class EditorInputConflictDetector
{
    /// <summary>
    /// 检测与现有绑定的冲突。
    /// </summary>
    /// <param name="existingBindings">现有绑定列表。</param>
    /// <param name="actionId">目标动作 ID。</param>
    /// <param name="newGesture">新手势。</param>
    /// <param name="conflictActionId">冲突的动作 ID。</param>
    /// <param name="conflictSlot">冲突的槽位。</param>
    /// <returns>是否冲突。</returns>
    public static bool DetectConflict(
        IReadOnlyList<EditorInputBinding> existingBindings,
        string actionId,
        EditorInputGesture newGesture,
        out string? conflictActionId,
        out string? conflictSlot)
    {
        conflictActionId = null;
        conflictSlot = null;

        if (newGesture is null) return false;

        var targetDef = Actions.EditorInputActionCatalog.FindById(actionId);
        if (targetDef is null) return false;

        var sig = newGesture.Signature;

        foreach (var binding in existingBindings)
        {
            if (binding.ActionId == actionId) continue;

            var otherDef = Actions.EditorInputActionCatalog.FindById(binding.ActionId);
            if (otherDef is null) continue;

            // 只有相同上下文才触发硬冲突
            if (otherDef.Context != targetDef.Context) continue;

            if (binding.PrimaryGesture?.Signature == sig)
            {
                conflictActionId = binding.ActionId;
                conflictSlot = "primary";
                return true;
            }
            if (binding.SecondaryGesture?.Signature == sig)
            {
                conflictActionId = binding.ActionId;
                conflictSlot = "secondary";
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 检查手势是否属于保留输入（无修饰左键/右键）。
    /// </summary>
    public static bool IsReservedGesture(EditorInputGesture gesture)
    {
        if (gesture.Device == EditorInputDevice.Mouse)
        {
            if (gesture.Modifiers == EditorInputModifiers.None &&
                (gesture.Code == "Left" || gesture.Code == "Right"))
                return true;
        }
        return false;
    }
}
