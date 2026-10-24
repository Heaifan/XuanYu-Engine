namespace XuanYu.Core.Space.Diagnostics;

internal sealed class TerrainErrorStatisticsBuilder(TerrainErrorOptions options)
{
    readonly List<double> errors=[]; int nodata,edge;
    public void Add(TerrainErrorGrid reference,int x,int y,double? surface,int edgeX,int edgeY)
    {
        if(edgeX<options.ExcludedSourceEdgeSamples||edgeY<options.ExcludedSourceEdgeSamples||
           edgeX>=reference.Width-options.ExcludedSourceEdgeSamples||edgeY>=reference.Height-options.ExcludedSourceEdgeSamples){edge++;return;}
        if(!reference.IsValid(x,y)||surface is null||!double.IsFinite(surface.Value)){nodata++;return;}
        errors.Add(global::System.Math.Abs(reference.At(x,y)-surface.Value));
    }
    public TerrainErrorStatistics Build()
    {
        var p50=Q(.5);var p95=Q(.95);var p99=Q(.99);var max=errors.Count==0?0:errors.Max();
        double? scale=options.EstimatedPixelsPerMeter??options.EstimatedPixelsPerWorldUnit;
        return new(errors.Count,nodata,edge,p50,p95,p99,max,
            p50*options.VerticalExaggeration,p95*options.VerticalExaggeration,
            p99*options.VerticalExaggeration,max*options.VerticalExaggeration,
            scale.HasValue?p95*options.VerticalExaggeration*scale:null,scale.HasValue?max*options.VerticalExaggeration*scale:null);
    }
    double Q(double q)
    {
        if (errors.Count == 0) return 0;
        var ordered = errors.Order().ToArray();
        return ordered[(int)global::System.Math.Ceiling(q * ordered.Length) - 1];
    }
    public static void Validate(TerrainErrorGrid g)
    {
        ArgumentNullException.ThrowIfNull(g);
        ArgumentNullException.ThrowIfNull(g.Values);
        ArgumentNullException.ThrowIfNull(g.NoData);
        var cells = (long)g.Width * g.Height;
        if (g.Width < 2 || g.Height < 2 ||
            g.Values.Count != cells || g.NoData.Count != cells)
            throw new ArgumentException("Invalid terrain error grid.", nameof(g));
    }
}
