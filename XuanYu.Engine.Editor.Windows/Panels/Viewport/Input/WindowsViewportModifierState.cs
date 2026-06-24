using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.Input;

/// <summary>
/// 修饰键（Ctrl / Shift / Alt）状态跟踪。
/// 供 WindowsViewportInputTranslator 协调使用。
/// </summary>
public sealed class WindowsViewportModifierState
{
    private EditorInputModifiers _currentModifiers;

    /// <summary>当前修饰键状态（用于外部查询）。</summary>
    public EditorInputModifiers CurrentModifiers => _currentModifiers;

    /// <summary>
    /// 更新修饰键状态。
    /// </summary>
    public void UpdateModifierState(int vk, bool pressed)
    {
        if (vk == VkShift)
            _currentModifiers = pressed
                ? _currentModifiers | EditorInputModifiers.Shift
                : _currentModifiers & ~EditorInputModifiers.Shift;
        else if (vk == VkControl)
            _currentModifiers = pressed
                ? _currentModifiers | EditorInputModifiers.Control
                : _currentModifiers & ~EditorInputModifiers.Control;
        else if (vk == VkMenu)
            _currentModifiers = pressed
                ? _currentModifiers | EditorInputModifiers.Alt
                : _currentModifiers & ~EditorInputModifiers.Alt;
    }

    /// <summary>是否为修饰键。</summary>
    public static bool IsModifierKey(int vk) =>
        vk == VkShift || vk == VkControl || vk == VkMenu;

    /// <summary>
    /// 重置修饰键状态（焦点丢失时调用，防止修饰键卡键）。
    /// </summary>
    public void Reset() => _currentModifiers = EditorInputModifiers.None;

    // Win32 VK 常量
    private const int VkShift = 0x10;
    private const int VkControl = 0x11;
    private const int VkMenu = 0x12;
}
