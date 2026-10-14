using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

public sealed class XycStdioHost(XycCommandProcessor processor)
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task RunAsync(TextReader input, TextWriter output,
        Action closeSession, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await input.ReadLineAsync(cancellationToken);
                if (line is null) break;
                XycResponse response;
                try
                {
                    var request = JsonSerializer.Deserialize<XycRequest>(line, Json) ??
                        throw new JsonException();
                    response = processor.Execute(request);
                }
                catch (JsonException)
                {
                    response = new("", false, "INVALID_INPUT", null, null, null, 0);
                }
                await output.WriteLineAsync(JsonSerializer.Serialize(response, Json));
                await output.FlushAsync(cancellationToken);
                if (response.Success && response.ObservedState is XycSessionClosed { Closed: true })
                    break;
            }
        }
        catch (OperationCanceledException) { }
        finally { closeSession(); }
    }
}
