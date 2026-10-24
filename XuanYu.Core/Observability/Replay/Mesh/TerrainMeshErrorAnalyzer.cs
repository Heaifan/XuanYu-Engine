namespace XuanYu.Core.Space.Diagnostics;

public static class TerrainMeshErrorAnalyzer
{
    public static TerrainMeshErrorReport Analyze(TerrainErrorGrid source, TerrainErrorGrid render,
        IReadOnlyList<TerrainErrorChunk> chunks, TerrainErrorOptions? options = null,
        int skirtVerticesExcluded = 0)
    {
        ArgumentNullException.ThrowIfNull(source); ArgumentNullException.ThrowIfNull(render);
        ArgumentNullException.ThrowIfNull(chunks); options ??= new();
        TerrainErrorStatisticsBuilder.Validate(source); TerrainErrorStatisticsBuilder.Validate(render);
        if (!double.IsFinite(options.VerticalExaggeration) || options.VerticalExaggeration <= 0 ||
            options.ExcludedSourceEdgeSamples < 0 || skirtVerticesExcluded < 0)
            throw new ArgumentOutOfRangeException(nameof(options));
        var sourceRender = new TerrainErrorStatisticsBuilder(options);
        for (var y=0; y<source.Height; y++) for (var x=0; x<source.Width; x++)
        {
            var (rx,ry)=Map(source,render,x,y,options);
            sourceRender.Add(source,x,y,TerrainErrorSurface.BilinearAt(render,rx,ry),x,y);
        }
        var surfaces = chunks.Select(c => new TerrainErrorSurface(render,c)).ToArray();
        var perChunk = new List<TerrainErrorChunkResult>(chunks.Count);
        for (var i=0;i<chunks.Count;i++)
        {
            var c=chunks[i]; var stats=new TerrainErrorStatisticsBuilder(options);
            for(var y=c.StartY;y<=c.StartY+c.CellCountY;y++) for(var x=c.StartX;x<=c.StartX+c.CellCountX;x++)
                stats.Add(render,x,y,surfaces[i].At(x,y),x,y);
            perChunk.Add(new TerrainErrorChunkResult(c.ChunkX,c.ChunkY,c.Lod,c.Stride,stats.Build())
            { SourceToFinalMesh = CompareSourceChunk(source,render,surfaces[i],options) });
        }
        var owners = BuildOwners(render,chunks);
        var finalStats = new TerrainErrorStatisticsBuilder(options);
        for(var y=0;y<source.Height;y++) for(var x=0;x<source.Width;x++)
        {
            var (rx,ry)=Map(source,render,x,y,options); var cx=global::System.Math.Clamp((int)rx,0,render.Width-1);
            var cy=global::System.Math.Clamp((int)ry,0,render.Height-1); var owner=owners[cy*render.Width+cx];
            finalStats.Add(source,x,y,owner<0?null:surfaces[owner].At(rx,ry),x,y);
        }
        return new(sourceRender.Build(),perChunk,finalStats.Build(),skirtVerticesExcluded,
            options.EstimatedPixelsPerMeter.HasValue||options.EstimatedPixelsPerWorldUnit.HasValue ? "estimated":"unavailable");
    }

    static int[] BuildOwners(TerrainErrorGrid grid,IReadOnlyList<TerrainErrorChunk> chunks)
    {
        var owners=Enumerable.Repeat(-1,grid.Width*grid.Height).ToArray();
        for(var i=0;i<chunks.Count;i++) { var c=chunks[i];
            for(var y=c.StartY;y<=c.StartY+c.CellCountY;y++) for(var x=c.StartX;x<=c.StartX+c.CellCountX;x++) owners[y*grid.Width+x]=i;
        }
        return owners;
    }

    static TerrainErrorStatistics CompareSourceChunk(TerrainErrorGrid source,TerrainErrorGrid render,
        TerrainErrorSurface surface,TerrainErrorOptions options)
    {
        var c=surface.Chunk; var scaleX=(source.Width-1d)/(render.Width-1d);
        var scaleY=(source.Height-1d)/(render.Height-1d);
        var startX=global::System.Math.Max(0,(int)global::System.Math.Ceiling(c.StartX*scaleX));
        var endX=global::System.Math.Min(source.Width-1,(int)global::System.Math.Floor((c.StartX+c.CellCountX)*scaleX));
        var lowY=options.SourceRowsSouthToNorth?render.Height-1-c.StartY-c.CellCountY:c.StartY;
        var highY=options.SourceRowsSouthToNorth?render.Height-1-c.StartY:c.StartY+c.CellCountY;
        var startY=global::System.Math.Max(0,(int)global::System.Math.Ceiling(lowY*scaleY));
        var endY=global::System.Math.Min(source.Height-1,(int)global::System.Math.Floor(highY*scaleY));
        var stats=new TerrainErrorStatisticsBuilder(options);
        for(var y=startY;y<=endY;y++) for(var x=startX;x<=endX;x++)
        {
            var (rx,ry)=Map(source,render,x,y,options);
            stats.Add(source,x,y,surface.At(rx,ry),x,y);
        }
        return stats.Build();
    }

    static (double X,double Y) Map(TerrainErrorGrid source,TerrainErrorGrid render,int x,int y,TerrainErrorOptions o) =>
        (x*(render.Width-1d)/(source.Width-1d), (o.SourceRowsSouthToNorth?source.Height-1d-y:y)*(render.Height-1d)/(source.Height-1d));
}
