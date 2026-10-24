namespace XuanYu.Core.Space.Diagnostics;

internal sealed class TerrainErrorSurface
{
    readonly TerrainErrorGrid field;
    readonly int[] xs,ys;
    public TerrainErrorChunk Chunk { get; }
    public TerrainErrorSurface(TerrainErrorGrid field,TerrainErrorChunk chunk)
    {
        this.field=field; Chunk=chunk;
        if(chunk.Lod<0||chunk.Lod>30||chunk.Stride!=(1<<chunk.Lod)||
           chunk.StartX<0||chunk.StartY<0||chunk.CellCountX<=0||chunk.CellCountY<=0||
           (long)chunk.StartX+chunk.CellCountX>=field.Width||
           (long)chunk.StartY+chunk.CellCountY>=field.Height)
            throw new ArgumentOutOfRangeException(nameof(chunk));
        xs=Coordinates(chunk.StartX,chunk.CellCountX,chunk.Stride); ys=Coordinates(chunk.StartY,chunk.CellCountY,chunk.Stride);
    }
    public bool Contains(double x,double y) =>
        x>=Chunk.StartX&&x<=Chunk.StartX+(double)Chunk.CellCountX&&
        y>=Chunk.StartY&&y<=Chunk.StartY+(double)Chunk.CellCountY;
    public double? At(double x,double y)
    {
        if(!Contains(x,y))return null; var ix=Find(xs,x);var iy=Find(ys,y);if(ix<0||iy<0)return null;
        var x0=xs[ix];var x1=xs[ix+1];var y0=ys[iy];var y1=ys[iy+1];
        if(!field.IsValid(x0,y0)||!field.IsValid(x1,y0)||!field.IsValid(x0,y1)||!field.IsValid(x1,y1))return null;
        var tx=(x-x0)/(x1-x0);var ty=(y-y0)/(y1-y0);
        // Matches builder triangles [TL,BL,TR] and [TR,BL,BR].
        return tx+ty<=1?(1-tx-ty)*field.At(x0,y0)+ty*field.At(x0,y1)+tx*field.At(x1,y0):
            (1-ty)*field.At(x1,y0)+(1-tx)*field.At(x0,y1)+(tx+ty-1)*field.At(x1,y1);
    }
    public static double? BilinearAt(TerrainErrorGrid field,double x,double y)
    {
        if(!double.IsFinite(x)||!double.IsFinite(y)||
           x<0||y<0||x>field.Width-1||y>field.Height-1) return null;
        var x0=(int)global::System.Math.Floor(x);
        var y0=(int)global::System.Math.Floor(y);
        var x1=global::System.Math.Min(x0+1,field.Width-1);var y1=global::System.Math.Min(y0+1,field.Height-1);
        if(!field.IsValid(x0,y0)||!field.IsValid(x1,y0)||!field.IsValid(x0,y1)||!field.IsValid(x1,y1))return null;
        var tx=x-x0;var ty=y-y0;
        return (1-ty)*((1-tx)*field.At(x0,y0)+tx*field.At(x1,y0))+
            ty*((1-tx)*field.At(x0,y1)+tx*field.At(x1,y1));
    }
    static int[] Coordinates(int start,int cells,int stride)
    {
        var a=new List<int>();
        for(long i=0;i<cells;i+=stride)a.Add(start+(int)i);
        if(a.Count==0||a[^1]!=start+cells)a.Add(start+cells);
        return a.ToArray();
    }
    static int Find(int[] a,double p)
    {
        var lo=0;var hi=a.Length-1;
        while(lo<hi-1)
        {
            var mid=(lo+hi)/2;
            if(p<=a[mid])hi=mid;else lo=mid;
        }
        return p>=a[lo]&&p<=a[hi]?lo:-1;
    }
}
