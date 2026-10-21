using System.Text.Json;
using System.Text.Json.Serialization;

namespace XuanYu.Render.Abstractions.Diagnostics;

public static class RendererEventJson
{
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
