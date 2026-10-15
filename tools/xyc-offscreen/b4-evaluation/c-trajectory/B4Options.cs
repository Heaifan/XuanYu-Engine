internal sealed record B4Options(string Source, string Trajectory, string Output,
    string Variant, string ErrorTable, string Head, string Dirty)
{
    public static B4Options Parse(string[] args)
    {
        var values = args.Chunk(2).ToDictionary(x => x[0], x => x[1], StringComparer.OrdinalIgnoreCase);
        string Get(string key) => Path.GetFullPath(values["--" + key]);
        var variant = values["--variant"];
        if (variant is not ("baseline" or "sse-4")) throw new ArgumentException("Variant must be baseline or sse-4");
        return new(Get("source"), Get("trajectory"), Get("out"), variant,
            Get("error-table"), values["--head"], values["--dirty"]);
    }
}
