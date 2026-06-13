using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using FluidWarfare.Editor.Input.Runtime;

namespace FluidWarfare.Tests.Editor.Input.Runtime;

public sealed class EditorInputBindingSnapshotTests
{
    [Fact]
    public void Build_WithBlenderPreset_ResolvesOrbit()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        var action = snapshot.Resolve("|MouseDrag|Mouse|Middle");
        Assert.NotNull(action);
        Assert.Equal("viewport.orbit", action.Id);
    }

    [Fact]
    public void Build_WithBlenderPreset_ResolvesPan()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        var action = snapshot.Resolve("Shift|MouseDrag|Mouse|Middle");
        Assert.NotNull(action);
        Assert.Equal("viewport.pan", action.Id);
    }

    [Fact]
    public void Build_WithBlenderPreset_ResolvesZoom()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        var action = snapshot.Resolve("|MouseWheel|Wheel|Y");
        Assert.NotNull(action);
        Assert.Equal("viewport.zoom", action.Id);
    }

    [Fact]
    public void Build_WithBlenderPreset_ResolvesFrameAll()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        var action = snapshot.Resolve("|KeyPress|Keyboard|Home");
        Assert.NotNull(action);
        Assert.Equal("viewport.frame_all", action.Id);
    }

    [Fact]
    public void Build_WithOverride_ReplacesPrimary()
    {
        var set = EditorInputActionCatalog.BlenderPreset with
        {
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "viewport.orbit",
                    Slot = "primary",
                    Gesture = new EditorInputGesture(
                        EditorInputDevice.Mouse, "Right",
                        EditorInputModifiers.Alt,
                        EditorInputGestureKind.MouseDrag)
                }
            }
        };

        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        // 旧手势不再解析
        Assert.Null(snapshot.Resolve("|MouseDrag|Mouse|Middle"));

        // 新手势生效
        var action = snapshot.Resolve("Alt|MouseDrag|Mouse|Right");
        Assert.NotNull(action);
        Assert.Equal("viewport.orbit", action.Id);
    }

    [Fact]
    public void Build_WithOverride_ClearsSlot()
    {
        var set = EditorInputActionCatalog.BlenderPreset with
        {
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "viewport.orbit",
                    Slot = "primary",
                    Gesture = null
                }
            }
        };

        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        // 旧手势不再解析
        Assert.Null(snapshot.Resolve("|MouseDrag|Mouse|Middle"));
    }

    [Fact]
    public void BeginDrag_LocksAction_AndGetActiveDragActionId_ReturnsIt()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        snapshot.BeginDrag("|MouseDrag|Mouse|Middle", 100, 200);

        var actionId = snapshot.GetActiveDragActionId();
        Assert.Equal("viewport.orbit", actionId);
    }

    [Fact]
    public void EndDrag_ClearsActiveDrag()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        snapshot.BeginDrag("|MouseDrag|Mouse|Middle", 100, 200);
        snapshot.EndDrag();

        Assert.Null(snapshot.GetActiveDragActionId());
    }

    [Fact]
    public void UnrecognizedSignature_ReturnsNull()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        Assert.Null(snapshot.Resolve("None|KeyPress|Keyboard|NonexistentKey"));
    }

    [Fact]
    public void WheelGesture_DefaultPresetSignature_MatchesTranslatorSignature()
    {
        // 强制不变量：Blender 默认滚轮手势的签名
        // 必须与 WindowsViewportInputTranslator 运行时生成的完全一致。
        // EditorInputGesture.BuildSignature 对 None mod 使用 ""（空串），
        // Translator 之前用 "0" 导致 |... vs 0|... 签名不匹配。

        var wheelBinding = EditorInputActionCatalog.BlenderDefaultBindings
            .First(b => b.ActionId == "viewport.zoom");
        Assert.NotNull(wheelBinding.PrimaryGesture);

        var defaultSig = wheelBinding.PrimaryGesture.Signature;

        // Translator 对无修饰滚轮构造的签名
        var translatorSig = BuildSignature(
            EditorInputDevice.Wheel, "Y",
            EditorInputGestureKind.MouseWheel,
            EditorInputModifiers.None);

        Assert.Equal(defaultSig, translatorSig);
    }

    // 镜像 WindowsViewportInputTranslator.BuildSignature 的签名算法
    private static string BuildSignature(EditorInputDevice device, string code,
        EditorInputGestureKind kind, EditorInputModifiers modifiers)
    {
        var mod = modifiers == EditorInputModifiers.None ? "" : modifiers.ToString();
        return $"{mod}|{kind}|{device}|{code}";
    }

    [Fact]
    public void WheelGesture_NoModifier_ProducesSameSignatureAsDefaultBinding()
    {
        // 验证 Blender 默认滚轮手势与无修饰滚轮产生的签名一致
        var defaultBinding = EditorInputActionCatalog.BlenderDefaultBindings
            .First(b => b.ActionId == "viewport.zoom");
        var gesture = new EditorInputGesture(
            EditorInputDevice.Wheel, "Y",
            EditorInputModifiers.None,
            EditorInputGestureKind.MouseWheel);
        Assert.Equal(defaultBinding.PrimaryGesture!.Signature, gesture.Signature);
    }

    [Fact]
    public void WheelGesture_WithControlModifier_ProducesCorrectSignature()
    {
        // Ctrl+滚轮应产生不同于无修饰滚轮的签名
        var plainGesture = new EditorInputGesture(
            EditorInputDevice.Wheel, "Y",
            EditorInputModifiers.None,
            EditorInputGestureKind.MouseWheel);
        var ctrlGesture = new EditorInputGesture(
            EditorInputDevice.Wheel, "Y",
            EditorInputModifiers.Control,
            EditorInputGestureKind.MouseWheel);
        Assert.NotEqual(plainGesture.Signature, ctrlGesture.Signature);
        Assert.Contains("Control", ctrlGesture.Signature);
    }

    [Fact]
    public void WheelDelta_Positive_ProducesPositiveValue()
    {
        // 滚轮向上 = 正 delta = 拉近
        var match = new EditorInputMatch
        {
            ActionId = "viewport.zoom",
            ValueKind = EditorInputValueKind.WheelDelta,
            WheelDelta = 120.0f / 120.0f // 1 notch
        };
        Assert.True(match.WheelDelta > 0);
    }

    [Fact]
    public void WheelDelta_Negative_ProducesNegativeValue()
    {
        // 滚轮向下 = 负 delta = 拉远
        var match = new EditorInputMatch
        {
            ActionId = "viewport.zoom",
            ValueKind = EditorInputValueKind.WheelDelta,
            WheelDelta = -120.0f / 120.0f // -1 notch
        };
        Assert.True(match.WheelDelta < 0);
    }

    [Fact]
    public void UnmodifiedOrbitSignature_MatchesTranslatorFormat()
    {
        // 强制不变量：无修饰 MMB 拖动（Orbit）的签名格式必须与 Translator 一致
        var defaultBinding = EditorInputActionCatalog.BlenderDefaultBindings
            .First(b => b.ActionId == "viewport.orbit");
        var gest = new EditorInputGesture(
            EditorInputDevice.Mouse, "Middle",
            EditorInputModifiers.None,
            EditorInputGestureKind.MouseDrag);
        Assert.Equal(defaultBinding.PrimaryGesture!.Signature, gest.Signature);
        Assert.StartsWith("|MouseDrag", gest.Signature); // None mod → 以 | 开头，不是 0|
    }

    [Fact]
    public void CtrlX_OpenPreferences_SignatureMatches()
    {
        // Ctrl+X 是用户确认实际生效的组合
        var defaultBinding = EditorInputActionCatalog.BlenderDefaultBindings
            .First(b => b.ActionId == "editor.open_preferences");
        var ctrlX = new EditorInputGesture(
            EditorInputDevice.Keyboard, "X",
            EditorInputModifiers.Control);
        var ctrlCommaSig = defaultBinding.PrimaryGesture!.Signature;
        var ctrlXSig = ctrlX.Signature;
        Assert.NotEqual(ctrlCommaSig, ctrlXSig);

        // 修改后 Ctrl+逗号 → Ctrl+X 能通过 Snapshot 解析
        var set = EditorInputActionCatalog.BlenderPreset with
        {
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "editor.open_preferences",
                    Slot = "primary",
                    Gesture = new EditorInputGesture(
                        EditorInputDevice.Keyboard, "X",
                        EditorInputModifiers.Control)
                }
            }
        };
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 1);

        // Ctrl+X 应解析为 open_preferences
        var result = snapshot.Resolve("Control|KeyPress|Keyboard|X");
        Assert.NotNull(result);
        Assert.Equal("editor.open_preferences", result.Id);

        // 旧 Ctrl+, 应不再解析
        Assert.Null(snapshot.Resolve("Control|KeyPress|Keyboard|Comma"));
    }

    [Fact]
    public void Revision_IsSetCorrectly()
    {
        var set = EditorInputActionCatalog.BlenderPreset;
        var snapshot = EditorInputBindingSnapshot.Build(set, EditorInputActionCatalog.All, 42);

        Assert.Equal(42, snapshot.Revision);
    }

    [Fact]
    public void BlenderDefaultBindings_AllActionsHaveGesture()
    {
        var bindings = EditorInputActionCatalog.BlenderDefaultBindings;

        // 17 个动作有默认主绑定（ground_placement.begin 没有默认绑定）
        Assert.Equal(17, bindings.Count);

        foreach (var b in bindings)
        {
            Assert.NotNull(b.PrimaryGesture);
        }
    }

    [Fact]
    public void SecondaryGesture_CanBeResolved()
    {
        // BlenderDefaultBindings 中没有定义 SecondaryGesture 的动作，
        // 但框架支持。创建一个含 secondary 的快照验证。
        var actions = EditorInputActionCatalog.All;
        var set = EditorInputActionCatalog.BlenderPreset with
        {
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "viewport.zoom",
                    Slot = "secondary",
                    Gesture = new EditorInputGesture(
                        EditorInputDevice.Keyboard, "Z",
                        EditorInputModifiers.Control,
                        EditorInputGestureKind.KeyPress)
                }
            }
        };

        var snapshot = EditorInputBindingSnapshot.Build(set, actions, 1);

        var action = snapshot.Resolve("Control|KeyPress|Keyboard|Z");
        Assert.NotNull(action);
        Assert.Equal("viewport.zoom", action.Id);
    }
}
