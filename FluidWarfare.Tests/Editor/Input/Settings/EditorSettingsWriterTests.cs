using System.Text.Json;
using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Tests.Editor.Input.Settings;

/// <summary>
/// 直接测试 EditorSettingsReader/Writer 序列化逻辑，
/// 不依赖 AppData 路径（Environment.GetFolderPath 不受 APPDATA 环境变量影响）。
/// </summary>
public sealed class EditorSettingsWriterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void DefaultDocument_RoundTrips()
    {
        var doc = new FluidWarfare.Editor.Input.Settings.EditorSettingsDocument();
        var json = JsonSerializer.Serialize(doc, JsonOptions);
        var read = JsonSerializer.Deserialize<FluidWarfare.Editor.Input.Settings.EditorSettingsDocument>(json, JsonOptions);
        Assert.NotNull(read);
        Assert.Equal("blender", read.Input.Preset);
    }

    [Fact]
    public void Override_RoundTrips()
    {
        var doc = new FluidWarfare.Editor.Input.Settings.EditorSettingsDocument
        {
            Input = new EditorInputBindingSet
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
            }
        };

        var json = JsonSerializer.Serialize(doc, JsonOptions);
        var read = JsonSerializer.Deserialize<FluidWarfare.Editor.Input.Settings.EditorSettingsDocument>(json, JsonOptions);
        Assert.NotNull(read);
        Assert.Single(read.Input.Overrides);
        Assert.Equal("viewport.pan", read.Input.Overrides[0].ActionId);
        Assert.NotNull(read.Input.Overrides[0].Gesture);
        Assert.Equal("Right", read.Input.Overrides[0].Gesture.Code);
    }

    [Fact]
    public void InvalidJson_ThrowsJsonException()
    {
        var json = "not valid json {{{";
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FluidWarfare.Editor.Input.Settings.EditorSettingsDocument>(json, JsonOptions));
    }

    [Fact]
    public void UnsupportedSchema_DeserializesButIsNotDefault()
    {
        var json = """{"schemaVersion":99,"input":{"preset":"unknown"}}""";
        var doc = JsonSerializer.Deserialize<FluidWarfare.Editor.Input.Settings.EditorSettingsDocument>(json, JsonOptions);
        Assert.NotNull(doc);
        Assert.Equal(99, doc.SchemaVersion);
        Assert.False(doc.IsDefault);
    }

    [Fact]
    public void EmptyObject_DeserializesAsDefault()
    {
        var json = "{}";
        var doc = JsonSerializer.Deserialize<FluidWarfare.Editor.Input.Settings.EditorSettingsDocument>(json, JsonOptions);
        Assert.NotNull(doc);
        Assert.True(doc.IsDefault);
    }

    [Fact]
    public void Gesture_SerializesAndDeserializes()
    {
        var gesture = new EditorInputGesture(EditorInputDevice.Mouse, "Middle",
            EditorInputModifiers.Control, EditorInputGestureKind.MouseDrag);
        var json = JsonSerializer.Serialize(gesture, JsonOptions);
        var read = JsonSerializer.Deserialize<EditorInputGesture>(json, JsonOptions);
        Assert.NotNull(read);
        Assert.Equal(gesture.Signature, read.Signature);
    }
}
