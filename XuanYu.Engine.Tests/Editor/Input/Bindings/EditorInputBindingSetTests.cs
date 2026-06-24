using System.Text.Json;
using XuanYu.Engine.Editor.Input.Bindings;

namespace FluidWarfare.Tests.Editor.Input.Bindings;

public sealed class EditorInputBindingSetTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void BindingSet_WithOverrides_RoundTrips()
    {
        var set = new EditorInputBindingSet
        {
            Preset = "blender",
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "viewport.pan",
                    Slot = "primary",
                    Gesture = new EditorInputGesture(
                        EditorInputDevice.Mouse, "Right",
                        EditorInputModifiers.Shift,
                        EditorInputGestureKind.MouseDrag)
                }
            }
        };

        var json = JsonSerializer.Serialize(set, JsonOptions);
        var read = Assert.IsType<EditorInputBindingSet>(
            JsonSerializer.Deserialize<EditorInputBindingSet>(json, JsonOptions));
        Assert.Equal("blender", read.Preset);
        Assert.Single(read.Overrides);
        Assert.Equal("viewport.pan", read.Overrides[0].ActionId);
        Assert.Equal("primary", read.Overrides[0].Slot);
        var gesture = Assert.IsType<EditorInputGesture>(read.Overrides[0].Gesture);
        Assert.Equal("Right", gesture.Code);
    }

    [Fact]
    public void BindingSet_EmptyOverrides_IsDefault()
    {
        var set = new EditorInputBindingSet
        {
            Preset = "blender",
            Overrides = Array.Empty<EditorInputBindingOverride>()
        };

        var json = JsonSerializer.Serialize(set, JsonOptions);
        var read = JsonSerializer.Deserialize<EditorInputBindingSet>(json, JsonOptions);

        Assert.NotNull(read);
        Assert.Empty(read.Overrides);
    }

    [Fact]
    public void BindingSet_ClearOverride_RoundTrips()
    {
        var set = new EditorInputBindingSet
        {
            Preset = "blender",
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "viewport.orbit",
                    Slot = "primary",
                    Gesture = null // 清除绑定
                }
            }
        };

        var json = JsonSerializer.Serialize(set, JsonOptions);
        var read = JsonSerializer.Deserialize<EditorInputBindingSet>(json, JsonOptions);

        Assert.NotNull(read);
        Assert.Single(read.Overrides);
        Assert.Null(read.Overrides[0].Gesture);
    }
}
